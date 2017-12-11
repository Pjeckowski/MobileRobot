/*
 * radio_rec.c
 *
 *  Created on: 8 mar 2017
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
#define FORW 	0
#define BACK 	1

//robot positions
double posX = 0, posY = 0, angle = 0;
double goalPosX = 0, goalPosY = 0;

//radio data
uint8_t radioSendBufor[5];
uint8_t radioRecBufor[5];

//robot control
uint8_t lEngineFill = 0, rEngineFill = 0;
uint8_t aLEngineFill = 0, aREngineFill = 0;
uint8_t lEngineDir = 0, rEngineDir = 0;
uint8_t isFollowingPoint = 0, isFollowingLine = 0;
uint8_t maxEngineFill = 0;


char *table = "a";
char status;

void radioRec();
uint8_t dataWorkout();
uint8_t* getTransmitData(uint8_t request);
void sendInfo();

volatile uint8_t irCounter;
int suppCounter = 0;
enum nrfState {REC, WFBT, WFBR, TRA1, TRA2, WFTR, WFRE};
enum nrfState RadioState = REC;
uint8_t radio_actionTimer = 0;

int main()
{
	//PA0 jako wejscie podci¹gniête do VCC
	DDRA = 0b00000000;
	PORTA = 0b00000001;
	_delay_ms(1000);
	init_SPI(1, 0); //master, fosc/128
	_delay_ms(1000);
	init_radio(1, 0, 5);//receiver
	_delay_ms(100);
	uart_init();

	TCCR0 |= (1 << CS02);
	TIMSK |= (1 << TOIE0);
	sei();
	_delay_ms(100);
	table = "Initialization";
	uart_sendString(table);
	uart_sendByteAsChar(get_reg(STATUS));
	_delay_ms(2000);
	PORTB |= (1 << CE);


	while(1)
	{
		radioRec();
    }
}

ISR(TIMER0_OVF_vect)
{
	TCNT0 = time1ms;
	irCounter ++;
	suppCounter ++;
	if(suppCounter == 1000)
	{
		posX++;
		posY++;
		angle++;
		aREngineFill++;
		aLEngineFill ++;
		if(aREngineFill == 100)
		{
			aREngineFill = aLEngineFill = 0;
		}
	}
}

void radioRec()
{
	switch(RadioState)
	{
		case REC:
		{
			if(!(IRQPIN & (1 << IRQ)))
			{
				PORTB &= ~(1 << CE);
				while(get_reg(STATUS) != 0b00001110)
				{
					radio_receive(radioRecBufor, 5);
				}
				sendInfo();
				if(dataWorkout(radioRecBufor)) //if transmiter wants respond
				{
					radio_switchTransmiter();
					RadioState = WFBT;
					radio_actionTimer = irCounter + 10;
				}
				else
				{
					PORTB |= (1 << CE);
				}
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
		case TRA1:
		{
			uart_sendString("Gonna transmit!");
			radio_preparePayload(radioSendBufor, 5);
			radio_actionTimer = irCounter + 10;
			RadioState = TRA2;
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
			if((!(IRQPIN & (1 <<IRQ))) || irCounter == radio_actionTimer)
			{
				if(get_reg(STATUS) == 0b00100000)
				{
					table = "\tTransmit succesfull!";
					uart_sendString(table);
				}
				uart_sendString("after transmition:");
				uart_sendByteAsChar(get_reg(STATUS));
				reset_radio();
				radio_switchReceiver();
				radio_actionTimer = irCounter + 10;
				RadioState = WFBR;
			}
			break;
		}
		case WFBR:
		{
			if(irCounter == radio_actionTimer)
			{
				RadioState = REC;
				PORTB |= (1 << CE);
			}
			break;
		}
		case WFRE:
		{
			break;
		}
	}
}

void sendInfo()
{
	table = "\tReceived!";
	uart_sendString(table);
	uart_sendByteAsChar(radioRecBufor[0]);
}

uint8_t dataWorkout(uint8_t* data)
{
	switch(data[0])
	{
		case GETX:
		{
			radioSendBufor[0] = GETX;
			getBytes(posX, radioSendBufor + 1);
			return 1;
		}
		case GETY:
		{
			radioSendBufor[0] = GETY;
			getBytes(posY, radioSendBufor + 1);
			return 1;
		}
		case GETA:
		{
			radioSendBufor[0] = GETA;
			getBytes(angle, radioSendBufor + 1);
			return 1;
		}
		case GETEF:
		{
			radioSendBufor[0] = GETEF;
			radioSendBufor[1] = aLEngineFill;
			radioSendBufor[2] = aREngineFill;
			return 1;
		}
		case SETGX:
		{
			goalPosX = getValFromBytes(data + 1);
			return 0;
		}
		case SETGY:
		{
			goalPosY = getValFromBytes(data + 1);
			return 0;
		}
		case SETEF:
		{
			lEngineFill = (data[2] & 0b011111111);
			rEngineFill = (data[3] & 0b011111111);

			if(data[2] & 0b10000000)
				lEngineDir = FORW;
			else
				lEngineDir = BACK;

			if(data[3] & 0b10000000)
				rEngineDir = FORW;
			else
				rEngineDir = BACK;

			return 0;
		}
		case FOTOP:
		{
			isFollowingPoint = 1;
			return 0;
		}
		case FOLIN:
		{
			isFollowingLine = 1;
			return 0;
		}
		case SETME:
		{
			maxEngineFill = data[2];
			return 0;
		}
		case COPAC:
		{
			return 0;
		}
		case RSTOP:
		{
			rEngineFill = lEngineFill = 0;
			isFollowingLine = isFollowingPoint = 0;
			return 0;
		}
		default:
		{
			return 0;
		}
	}
}

uint8_t* getTransmitData(uint8_t request)
{
	radioRecBufor[0] += 1;
	return radioRecBufor;
}

