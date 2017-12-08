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

void errorInform();
void dataWorkout(uint8_t data[], uint8_t count);
void radioTra();
void dataWorkout1();

volatile uint8_t irCounter;
enum nrfState {REC, WFBT, WFBR, TRA1, TRA2, WFTR, WFRE};
enum nrfState RadioState = TRA1;
volatile uint8_t radio_actionTimer = 0;
volatile uint8_t transmitTrigger = 1;
char *table = "a";

//radio data
uint8_t radioSendBufor[5];
uint8_t radioRecBufor[5];

//radio positions
double posX = 0, posY = 0, angle = 0;
double goalPosX = 0, goalPosY = 0;

volatile uint16_t counter=0;

int main()
{
	DDRD=0b01000000;
	//PA0 jako wejscie podci¹gniête do VCC
	DDRA = 0b00000000;
	PORTA = 0b00000001;

	TCCR0 |= (1 << CS02);
	TIMSK |= (1 << TOIE0);
	sei();

	_delay_ms(3000);
	init_SPI(1,0);
	init_radio(0, 0, 5);

	if(get_reg(STATUS) == 0b00001110)
		PORTD = 0b01000000;
	_delay_ms(5000);

	PORTD = 0b00000000;
	radioRecBufor[0] = 0;

	uart_init();

	while(1)
	{
		radioTra();
	}
}

ISR(TIMER0_OVF_vect)
{
	TCNT0 = time1ms;
	irCounter ++;
	counter ++;
	if(counter == 999)
	{
		counter = 0;
		transmitTrigger = 1;

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
				posX += 1;
				radioSendBufor[0] = SETGX;
				getBytes(posX, radioSendBufor + 1);
				radio_preparePayload(radioSendBufor,5);
				radio_actionTimer = irCounter + 10;
				transmitTrigger = 0;
				RadioState = TRA2;
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
				uart_sendString("Transmitted!");
				uart_sendByteAsChar(get_reg(STATUS));
				if(get_reg(STATUS) == 0b00101110)//if transmition succesfull
				{
					if(radioSendBufor[0] & 0x80) //if i want respond
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
						uart_sendString("Received!");
						uart_sendByteAsChar(get_reg(STATUS));
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
						radio_actionTimer = irCounter + 10;
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

}

void dataWorkout1()
{
	table = "\tReceived!";
	uart_sendString(table);
	uart_sendByteAsChar(radioRecBufor[0]);
}






