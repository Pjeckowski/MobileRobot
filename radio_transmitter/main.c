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
#define IRQ 	PA0
#define IRQPIN	PINA
#define time1ms 255 -31
#define UART 	1
#define POSUB	0

void errorInform();
void dataWorkout(uint8_t data[], uint8_t count);
void radioTra();
void dataWorkout1();
uint8_t getNextPosRequest();
void uartRequestCollect(uint8_t data);
void prepareNextTransmission();

volatile uint8_t irCounter;
enum nrfState {REC, WFBT, WFBR, TRA1, TRA2, WFTR, WFRE};
enum nrfState RadioState = TRA1;
volatile uint8_t radio_actionTimer = 0;
volatile uint8_t transmitTrigger = 1;
volatile uint8_t radioBusy = 0, lastSent = POSUB;
uint8_t nextCPTimer = 0;
char *table = "a";

//radio data
volatile uint8_t radioSendBufor[5];
volatile uint8_t radioRecBufor[5];

//radio positions
double posX = 0, posY = 0, angle = 0;
double goalPosX = 0, goalPosY = 0;

//uart connection
volatile uint8_t uartHeaderReceived = 0;
volatile uint8_t uartSendBufor[5];
uint8_t uartRecCounter = 0, uartTransmitTrigger = 0, uartFlushTimer = 0, isPOSU = 1;
volatile uint8_t posuPosition = 0;

volatile uint16_t counter=0;
float a = 6000;

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
	init_SPI(1,0);
	init_radio(0, 0, 5);

	if(get_reg(STATUS) == 0b00001110)
		PORTD = 0b01000000;
	_delay_ms(3000);

	PORTD = 0b00000000;
	radioRecBufor[0] = 0;

	uart_init();

	while(1)
	{
		prepareNextTransmission();
		radioTra();
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
	if(0!= uartFlushTimer && irCounter == uartFlushTimer)
	{
		uartRecCounter = 0;
		uartFlushTimer = 0;
		uartHeaderReceived = 0;
	}
}

void prepareNextTransmission()
{
	if(radioBusy == 0)
	{
		if(1 == uartTransmitTrigger && 1 == isPOSU)
		{
			if(lastSent == UART)
			{
				radioSendBufor[0] = getNextPosRequest();
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
				lastSent = UART;
			}
			else
				if(isPOSU)
				{
					radioSendBufor[0] = getNextPosRequest();
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

void radioTra()
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
			if((!(IRQPIN & (1 << IRQ))) || irCounter == radio_actionTimer)
			{
				if(get_reg(STATUS) == 0b00101110)//if transmition succesfull
				{
					if(isRequest(radioSendBufor[0])) //if i want respond
					{
						reset_radio();
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
					RadioState = TRA1;
				}
			}
			break;
		}
		case WFBR:
				{
					if(irCounter == radio_actionTimer)
					{
						PORTB |= (1 << CE);
						radio_actionTimer = irCounter + 30;
						RadioState = WFRE;
					}
					break;
				}
		case WFRE:
				{
					if((!(IRQPIN & (1 << IRQ))) || irCounter == radio_actionTimer)
					{
						PORTB &= ~(1 << CE);
						if(get_reg(STATUS) == 0b01000000)//if receiving succesfull
						{
							while(get_reg(STATUS) != 0b00001110)
							{
								radio_receive(radioRecBufor, 5);
							}
							dataWorkout(radioRecBufor,5);
						}
						else
						{
							errorInform();
						}
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

void dataWorkout1()
{
	table = "\tReceived!";
	uart_sendString(table);
	uart_sendByteAsChar(radioRecBufor[0]);
}

void uartRequestCollect(uint8_t data)
{
	if(uartHeaderReceived)
	{
		if(uartRecCounter < 5)
		{
			if(data == '\r')
			{
				uartRecCounter = 0;
				uartTransmitTrigger = 1;
				uartFlushTimer = 0;
			}
			else
			{
				uartSendBufor[uartRecCounter] = data;
				uartRecCounter++;
				if(0 == (uartFlushTimer = irCounter + 5))
				{
					uartFlushTimer ++;
				}
			}
		}
		else
		{
			uartRecCounter = 0;
			uartHeaderReceived = 0;
			uartFlushTimer = 0;
		}
	}
	else
	{
		if(data == POSU)
		{
			isPOSU = 1;
		}
		else
		{
			int i;
			for(i = 0; i < headerCollectionLenght; i++)
			{
				if(data == headerCollection[i])
				{
					uartHeaderReceived = 1;
					uartSendBufor[0] = data;
					uartRecCounter = 1;
					if(0 == (uartFlushTimer = irCounter + 5))
					{
						uartFlushTimer ++;
					}
					break;
				}
			}
		}
	}
}

uint8_t getNextPosRequest()
{
	switch (posuPosition)
	{
		case GETX:
		{
			posuPosition  = GETY;
			return GETX;
		}
		case GETY:
		{
			posuPosition = GETA;
			return GETY;
		}
		case GETA:
		{
			posuPosition = GETEF;
			return GETA;
		}
		case GETEF:
		{
			posuPosition = GETX;
			return GETEF;
		}
		default:
		{
			posuPosition = GETX;
			return GETX;
		}
	}
}


