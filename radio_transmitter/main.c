/*
 * radio.c
 *
 *  Created on: 2 mar 2017
 *      Author: Patryk
 */

#include<avr/io.h>
#include<util/delay.h>
#include "nrf24l01\rf24l01.h"
#include "uart\my_uart.h"
#include "robot\engine.h"
#include "memops\memops.h"
#include <String.h>

#define Control_led PD6
#define time1ms 255 -31
#define UART 	1
#define POSUB	0

void errorInform();
void dataWorkout(uint8_t data[], uint8_t count);
void radioTransmit();
void uartRequestCollect(uint8_t data);
void radioPrepareNextTransmission();
void uartPacketWorkout(uint8_t* packet, uint8_t count);
uint8_t getNextPositionRequest();
float getXYValue(uint8_t* packet, uint8_t count);

volatile uint8_t irCounter;
enum nrfState {REC, WFBT, WFBR, TRA1, TRA2, WFTR, WFRE};
enum nrfState RadioState = TRA1;
volatile uint8_t radio_actionTimer = 0;
volatile uint8_t transmitTrigger = 1;
volatile uint8_t radioBusy = 0, lastSent = POSUB;
uint8_t nextCPTimer = 0;
char *table = "a";

//radio data
uint8_t radioSendBufor[5];
uint8_t radioRecBufor[5];

//radio positions
double posX = 0, posY = 0, angle = 0;
double goalPosX = 0, goalPosY = 0;

//uart connection
volatile uint8_t uartHeaderReceived = 0;
uint8_t uartSendBufor[5];
uint8_t uartRecBufor[100];
uint8_t uartRecCounter = 0, uartTransmitTrigger = 0, uartFlushTimer = 0, isRobotPositionSubscribed = 1;
volatile uint8_t positionRequest = 0;

volatile uint16_t counter=0;


int main()
{
	DDRD=0b01000000;
	//PA0 jako wejscie podci¹gniête do VCC
	DDRA = 0b00000000;
	PORTA = 0b00000001;


	_delay_ms(2000);

	TCCR0 |= (1 << CS02);
	TIMSK |= (1 << TOIE0);
	sei();

	_delay_ms(3000);
	SPI_init(1,0);
	radio_init(0, 0, 5);

	if(radio_ReadRegister(STATUS) == 0b00001110)
		PORTD = 0b01000000;

	_delay_ms(3000);
	PORTD = 0b00000000;
	radioRecBufor[0] = 0;

	uart_init();
	table = "Initialization";
	uart_sendString(table);
	uart_sendByteAsChar(radio_ReadRegister(STATUS));

	while(1)
	{
		radioPrepareNextTransmission();
		radioTransmit();
	}
}


ISR(TIMER0_OVF_vect)
{
	TCNT0 = time1ms;
	irCounter ++;
	counter ++;
	if(counter == 20)
	{
		counter = 0;
	}
	if(0 != uartFlushTimer && irCounter == uartFlushTimer)
	{
		uartRecCounter = 0;
		uartFlushTimer = 0;
		uartHeaderReceived = 0;
	}
}

ISR(USART_RXC_vect)
{
	uartRequestCollect(uart_receive());
}

void uartRequestCollect(uint8_t data)
{
	if(uartHeaderReceived)
	{
	     if(data == '\r')
	     {
	    	 uartPacketWorkout(uartRecBufor, uartRecCounter);
	    	 uartRecCounter = 0;
	    	 uartHeaderReceived = 0;
	     }
	     else
	     {
	    	 uartRecBufor[uartRecCounter] = data;
	    	 uartRecCounter ++;
	     }
	}
	else
	{
		if(data == 'P')
			uartHeaderReceived = 1;
	}
}

void uartPacketWorkout(uint8_t* packet, uint8_t count)
{
	switch (packet[0])
	{
		case USETX:
		{
			float val;
			val = getXYValue(packet, count);
			if(val == 40)
				PORTD ^= (1 << PD6);
			uartSendBufor[0] = SETGX;
			getBytes(val, uartSendBufor + 1);
			uartTransmitTrigger = 1;
			break;
		}
		case USETY:
		{
			float val;
			val = getXYValue(packet, count);
			uartSendBufor[0] = SETGY;
			getBytes(val, uartSendBufor + 1);
			uartTransmitTrigger = 1;
			break;
		}
		case UENGS:
		{
			PORTD ^= (1 << PD6);
			uartSendBufor[0] = SETEF;
			uartSendBufor[1] = packet[2];
			uartSendBufor[2] = packet[4];
			if(packet[1] != 0)
				uartSendBufor[1] |= 0x80;
			if(packet[3] != 0)
				uartSendBufor[2] |= 0x80;
			uartTransmitTrigger = 1;
			break;
		}
		case ULFOL:
		{
			uartSendBufor[0] = FOLIN;
			uartTransmitTrigger = 1;
			break;
		}
		case UPFOL:
		{
			uartSendBufor[0] = FOTOP;
			uartTransmitTrigger = 1;
			break;
		}
		case USTOP:
		{
			uartSendBufor[0] = RSTOP;
			uartTransmitTrigger = 1;
			break;
		}
		default:
		{
			break;
		}
	}
}

void radioPrepareNextTransmission()
{
	if(radioBusy == 0)
	{
		if(1 == uartTransmitTrigger && 1 == isRobotPositionSubscribed)
		{
			if(lastSent == UART)
			{
				radioSendBufor[0] = getNextPositionRequest();
				lastSent = POSUB;
				transmitTrigger = 1;
			}
			else
			{
				int i;
				for(i = 0; i < 5; i++)
				{
					radioSendBufor[i] = uartSendBufor[i];
					transmitTrigger = 1;
				}
				uartTransmitTrigger = 0;
				lastSent = UART;
			}
		}
		else
			if(uartTransmitTrigger)
			{
				int i;
				for(i = 0; i < 5; i++)
				{
					radioSendBufor[i] = uartSendBufor[i];
					transmitTrigger = 1;
				}
				uartTransmitTrigger = 0;
				lastSent = UART;
			}
			else
				if(isRobotPositionSubscribed)
				{
					radioSendBufor[0] = getNextPositionRequest();
					lastSent = POSUB;
					transmitTrigger = 1;
				}
				else
				{
					if(irCounter == nextCPTimer)
					{
						radioSendBufor[0] = COPAC;
						transmitTrigger = 1;
						nextCPTimer = irCounter + 200;
					}
				}
		}
}

void radioTransmit()
{
	switch (RadioState)
	{
		case TRA1:
		{
			if(transmitTrigger == 1)
			{
				radioBusy = 1;
				radio_preparePayload(radioSendBufor,5);
				radio_actionTimer = irCounter + 10;
				transmitTrigger = 0;
				RadioState = TRA2;
			}
			else
			{
				radioBusy = 0;
			}
			break;
		}
		case TRA2:
		{
			if(irCounter == radio_actionTimer)
			{
				radio_transmit();
				radio_actionTimer = irCounter + 10;
				RadioState = WFTR;
			}
			break;
		}
		case WFTR:
		{
			if(radio_isInterruptRequest() || irCounter == radio_actionTimer)
			{
				if(radio_wasTransmissionSuccessfull())
				{
					if(isRequest(radioSendBufor[0]))
					{
						radio_reset();
						radio_switchReceiver();
						radio_actionTimer = irCounter + 30;
						RadioState = WFBR;
					}
					else
					{
						RadioState = TRA1;
					}
				}
				else
				{
					radio_reset();
					radio_switchTransmiter();
					RadioState = WFBT;
				}
			}
			break;
		}
		case WFBR:
				{
					if(irCounter == radio_actionTimer)
					{
						radio_startListenning();
						radio_actionTimer = irCounter + 30;
						RadioState = WFRE;
					}
					break;
				}
		case WFRE:
				{
					if(radio_isInterruptRequest() || irCounter == radio_actionTimer)
					{
						radio_stopListenning();
						if(radio_wasDataReceived())//if receiving succesfull
						{
							while(!radio_isReceivingBuforEmpty())
							{
								radio_receive(radioRecBufor, 5);
							}
							dataWorkout(radioRecBufor,5);
						}
						else
						{
							errorInform();
						}

						radio_reset();
						radio_switchTransmiter();
						RadioState = WFBT;
						radio_actionTimer = irCounter + 25;
					}
					break;
				}
		case WFBT:
		{
			if(irCounter == radio_actionTimer)
			{
				RadioState = TRA1;
			}
			break;
		}
		case REC:
		{
			break;
		}

	}
}

void errorInform()
{
	table = "Error during receiving data";
	uart_sendString(table);
}

void dataWorkout(uint8_t data[], uint8_t count )
{
	uart_sendPacket(data, count);
}

float getXYValue(uint8_t* packet, uint8_t count)
{
	float value = 0;
	int multiply = 1;

	if(packet[1] == '-')
	{

		int i;
		for(i = count - 1; i > 1; i--)
		{
			value += (int)((packet[i] - 48) * multiply);
			multiply *= 10;
		}

		return -(value/100);
	}
	else
	{

		int i;
		for(i = count - 1; i > 0; i--)
		{
			value += (int)((packet[i] - 48) * multiply);
			multiply *= 10;
		}

		return (value/100);
	}
}

uint8_t getNextPositionRequest()
{
	switch (positionRequest)
	{
		case GETX:
		{
			positionRequest  = GETY;
			return GETX;
		}
		case GETY:
		{
			positionRequest = GETA;
			return GETY;
		}
		case GETA:
		{
			positionRequest = GETEF;
			return GETA;
		}
		case GETEF:
		{
			positionRequest = GETX;
			return GETEF;
		}
		default:
		{
			positionRequest = GETX;
			return GETX;
		}
	}
}


