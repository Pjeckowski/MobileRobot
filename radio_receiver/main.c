/*
 * radio_rec.c
 *
 *  Created on: 8 mar 2017
 *      Author: Patryk
 */

#include<avr/io.h>
#include<util/delay.h>
#include "rf24l01.h"
#include "uart\my_uart.h"
#include "robot\protocol.h"
#include "robot\engine.h"
#include "memops\memops.h"
#include <String.h>


#define Control_led PD6
#define time1ms 255 -31
#define FORW 	0
#define BACK 	1
#define PERIOD 50

//robot positions
float posX = 0, posY = 0, angle = 0;
float goalPosX = 0, goalPosY = 0;

//radio data
uint8_t radioSendBufor[5];
uint8_t radioRecBufor[5];
enum nrfState {REC, WFBT, WFBR, TRA1, TRA2, WFTR, RESET};
volatile enum nrfState RadioState = REC;
volatile uint8_t radioActionTimer = 0;
volatile uint8_t radioReceived = 0;
volatile uint8_t radioResetTimer = 0;

//robot control
int lEngineFill = 0, rEngineFill = 0;
int aLEngineFill = 0, aREngineFill = 0;
uint8_t lEngineDir = 0, rEngineDir = 0;
uint8_t isFollowingPoint = 0, isFollowingLine = 0;
uint8_t maxEngineFill = 0;


char *table = "a";
char status;

void SetEngines();
void ResetRadioIfNeeded();
void radioRec();
uint8_t dataWorkout();
void sendInfo();

volatile uint8_t irCounter, engCounter, wasInterrupt, wasOverflow;
int suppCounter = 0;

void setINT0risingEdgeInterrupt()
{
	MCUCR |= (1 << ISC00) | (1 << ISC01);
	//MCUCR|= (1 << ISC10) | (1 << ISC11);
	GICR |= (1 << INT0);
}

int main()
{
	//MCUCSR |= (1<<JTD);
	//MCUCSR |= (1<<JTD);

	_delay_ms(1000);
	SPI_init(1, 0); //master, fosc/128
	_delay_ms(1000);
	radio_init(1, 0, 5);//receiver, 5b danych
	_delay_ms(100);

	uart_init();
	engine_Init();

	TCCR0 |= (1 << CS02);
	TIMSK |= (1 << TOIE0);
	setINT0risingEdgeInterrupt();
	sei();
	_delay_ms(100);
	table = "Initialization";
	uart_sendString(table);
	uart_sendByteAsChar(radio_ReadRegister(STATUS));
	_delay_ms(2000);
	radio_startListenning();


	while(1)
	{
		radioRec();
		SetEngines();
		ResetRadioIfNeeded();
    }
}

ISR(INT0_vect)
{
	if(engine_IsEng1RotatingBack())
	PORTD |= (1 << PD6);
	else
	PORTD &= ~(1 << PD6);
}

ISR(TIMER0_OVF_vect)
{
	TCNT0 = time1ms;
	irCounter ++;
	wasInterrupt = 1;
	suppCounter ++;
	engCounter ++;
	if(engCounter > PERIOD)
	{
		engCounter = 1;
		wasOverflow++;
	}
	if(suppCounter >= 1000)
	{
		posX++;
		posY++;
		angle++;
		suppCounter = 0;
	}
}
void SetEngines()
{
	if(wasInterrupt)
	{
		wasInterrupt = 0;
		if(engCounter % 10 == 0)
		{
			if(aLEngineFill < lEngineFill)
			{
				if(aLEngineFill < 15)
				{
					aLEngineFill = 15;
				}
				else
				aLEngineFill++;
			}
			if(aREngineFill < rEngineFill)
			{
				if(aREngineFill < 15)
				{
					aREngineFill = 15;
				}
				else
					aREngineFill ++;
			}
			if(aLEngineFill > lEngineFill)
				aLEngineFill--;
			if(aREngineFill > rEngineFill)
				aREngineFill --;
		}


		if(engCounter <= aREngineFill)
		{
			if(rEngineDir == FORW)
				engine_Eng1DriveForward();
			else
				engine_Eng1DriveBack();
		}
		else
		{
			engine_Eng1Stop();
		}

		if(engCounter <= aLEngineFill)
		{
			if(lEngineDir == FORW)
				engine_Eng2DriveForward();
			else
				engine_Eng2DriveBack();
		}
		else
		{
			engine_Eng2Stop();
		}
	}
}

void ResetRadioIfNeeded()
{
	if(wasOverflow >= 2)
	{
		wasOverflow = 0;
		if(radioReceived)
		{
			radioReceived = 0;
			radioResetTimer = 0;
		}
		else
		{
			radioResetTimer ++;
		}
		if(radioResetTimer > 3)
		{
			RadioState = RESET;
		}
	}
}

int isTimeForAction()
{
	return irCounter == radioActionTimer ||
			irCounter == (radioActionTimer + 1) ||
			irCounter == (radioActionTimer + 2);
}

void setNextActionTime(int delayMS)
{
	radioActionTimer = irCounter + delayMS;
}

///Function manages state of the radio transceiver,
///sends and receives data.
void radioRec()
{
	switch(RadioState)
	{
		case REC:
		{
			if(radio_isInterruptRequest())
			{
				radioReceived = 1;
				radio_stopListenning();
				while(!radio_isReceivingBuforEmpty())
				{
					radio_receive(radioRecBufor, 5);
				}
				if(dataWorkout(radioRecBufor)) //if transmitter wants respond
				{
					radio_switchTransmiter();
					RadioState = WFBT;
					setNextActionTime(28);
				}
				else
				{
					radio_startListenning();
				}
			}
			break;
		}
		case WFBT:
		{
			if(isTimeForAction())
			{
				RadioState = TRA1;
			}
			break;
		}
		case TRA1:
		{
			radio_preparePayload(radioSendBufor, 5);
			setNextActionTime(10);
			RadioState = TRA2;
			break;
		}
		case TRA2:
		{
			if(isTimeForAction())
			{
				radio_transmit();
				setNextActionTime(10);
				RadioState = WFTR;
			}
			break;
		}
		case WFTR:
		{
			if(radio_isInterruptRequest() || isTimeForAction())
			{
				radio_reset();
				radio_switchReceiver();
				setNextActionTime(10);
				RadioState = WFBR;
			}
			break;
		}
		case WFBR:
		{
			if(isTimeForAction())
			{
				RadioState = REC;
				radio_startListenning();
			}
			break;
		}
		case RESET:
		{
			lEngineFill = rEngineFill = 0;
			isFollowingPoint = 0, isFollowingLine = 0;
			radio_reset();
			radio_init(1, 0, 5);//receiver, 5b data
			setNextActionTime(20);
			RadioState = WFBR;
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

///Function returns 1 if transmitter wants respond
///and 0 if it doesn't.
///If transmitter wants respond it also prepares the radioSendBuffor
///with proper data payload. If received packet is valid function
///fill connected variables like engine's fills or goal positions
///with data received in packet.
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
			posX = goalPosX = getValFromBytes(data + 1);//added posX to debug if connection works
			return 0;
		}
		case SETGY:
		{
			goalPosY = getValFromBytes(data + 1);
			return 0;
		}
		case SETEF:
		{
			float lEF = (float) (data[1] & 0b01111111);
			float rEF = (float) (data[2] & 0b01111111);
			lEF = lEF / 100.0 * (float) PERIOD;
			rEF = rEF / 100.0 * (float) PERIOD;
			lEngineFill = (int) lEF;
			rEngineFill = (int) rEF;
			if(lEngineFill)
			{
				if(data[1] & 0b10000000)
					lEngineDir = FORW;
				else
					lEngineDir = BACK;
			}
			if(rEngineFill)
			{
				if(data[2] & 0b10000000)
					rEngineDir = FORW;
				else
					rEngineDir = BACK;
			}
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

