/*
 * radio_rec.c
 *
 *  Created on: 8 mar 2017
 *      Author: Patryk
 */

#include<avr/io.h>
#include<util/delay.h>
#include "source\nrf24l01\rf24l01.h"
#include "source\robot\engine.h"
#include "source\memops\memops.h"
#include <stdlib.h>
#include <Math.h>
#include <util/atomic.h>
#include "source\rfSensor/rfSensor.h"
#include "source\uart\my_uart.h"
#include "source\robot\protocol.h"

#define CONT_LED PB0
#define LED_PORT PORTB
#define LED_DDR DDRB
#define time1ms 255 -62 //change clock basing on freq
#define PERIOD 50 //engine driving resolution (1 = 1ms)
#define D_WIDTH 15

//functions
void SendInfo();
void SetEngines();
void RadioReceive();
void SetRadioStateReset();
uint8_t ProcessDataFromRadio();

inline void ledON();
inline void ledOFF();
inline void ledToggle();

//radio data
uint8_t radioSendBufor[D_WIDTH];
uint8_t radioRecBufor[D_WIDTH];
enum nrfState {REC, WFBT, WFBR, TRA1, TRA2, WFTR, RESET};
volatile enum nrfState radioState = REC;
volatile uint8_t radioActionTimer = 0;
volatile uint8_t radioReceived = 0;
volatile uint8_t radioResetTimer = 0;

//robot control
int lEngineFill = 0, rEngineFill = 0;
int aLEngineFill = 0, aREngineFill = 0;
uint8_t isFollowingPoint = 0, isFollowingLine = 1, maxEngineFill = 0;

//lineFollowerVariables
ReflectiveSensor leftRS, rightRS;

//counters etc.
volatile uint8_t irCounter, engCounter, wasInterrupt, wasOverflow;
int suppCounter = 0;

//robot positions
volatile double posX = 0, posY = 0, angle = 0;
double goalPosX = 0, goalPosY = 0;
volatile double leftWheelSpeed, rightWheelSpeed;
double impulsesPerCircle = 341.2;
double wheelSpacing = 22.0;
volatile double wheelPerimeter = 0;
volatile double wheelSize = 8;
volatile double angleDiff = 0, robotSpeed = 0;
float angleDiffPart, robotSpeedPart;
volatile int rightEngSteps = 0, leftEngSteps = 0;
volatile int rightEngStepsC = 0, leftEngStepsC = 0;

int n = 3;

inline void CalculatePosition()
{
    ATOMIC_BLOCK(ATOMIC_FORCEON)
    {
      rightEngStepsC = rightEngSteps;
      leftEngStepsC = leftEngSteps;
      rightEngSteps = 0;
      leftEngSteps = 0;
    }
	leftWheelSpeed = wheelPerimeter * leftEngStepsC / impulsesPerCircle;
	rightWheelSpeed = wheelPerimeter * rightEngStepsC / impulsesPerCircle;
	//leftEngSteps = 0;
	//rightEngSteps = 0;

	robotSpeed = (leftWheelSpeed + rightWheelSpeed) / 2.0;
	angleDiff = (rightWheelSpeed - leftWheelSpeed) / wheelSpacing;

	n = 3;
	angleDiffPart = angleDiff / n;
	robotSpeedPart = robotSpeed / n ;

	int i = 0;
	for(; i < 3; i++)
	{
		posX += robotSpeedPart * cos(angle);
		posY += robotSpeedPart * sin(angle);
		angle += angleDiffPart;
		if(angle > 2 * M_PI)
			angle -= 2 * M_PI;
		if(angle < 0)
			angle = 2*M_PI + angle;
	}
}

void SetINT0INT1risingEdgeInterrupt()
{
	PORTD |= (1 << PD2) | (1 << PD3);
	MCUCR |= (1 << ISC00) | (1 << ISC01);
	MCUCR|= (1 << ISC10) | (1 << ISC11);
	GICR |= (1 << INT0) | (1 << INT1);
}

int main()
{
	_delay_ms(500); //delay for proper uC and components boot up

	//setting values required to calculate position
	angle = 90.0 * M_PI / 180.0;
	wheelPerimeter = wheelSize * M_PI;

	LED_DDR |= (1 << CONT_LED);

	spi_Init(1, 0); //master, fosc/128
	_delay_ms(100);
	radio_Init(1, 1, D_WIDTH);//receiver, 1Mb, 15b data
	engine_Init();

	TCCR0 |= (1 << CS02);
	TIMSK |= (1 << TOIE0);
	SetINT0INT1risingEdgeInterrupt();

	_delay_ms(100);
	 leftRS = refSensor_Init(5, 4, 3, 750);
	 rightRS = refSensor_Init(2, 1, 0, 750);

	_delay_ms(200);
	radio_StartListenning();

	sei();

	while(1)
	{
		RadioReceive();
		SetEngines();
		//SetRadioStateReset();
		CalculatePosition();
		if(isFollowingLine)
		{
			if(refSensor_AreValuesReady(&leftRS))
			{
				refSensor_Clean(&leftRS);
					if(leftRS.rsValues.LeftIn)
						ledON();
					else
						ledOFF();
			}
		}
    }
}

ISR(INT0_vect)
{
	if(engine_IsEng1RotatingBack())
	{
		rightEngSteps--;
		//ledON();
	}
	else
	{
		rightEngSteps++;
		//ledOFF();
	}
}

ISR(INT1_vect)
{
	if(engine_IsEng2RotatingBack())
		leftEngSteps--;
	else
		leftEngSteps++;
}

ISR(TIMER0_OVF_vect)
{
	TCNT0 = time1ms;
	irCounter ++;
	wasInterrupt = 1;
	engCounter ++;
	if(engCounter > PERIOD)
	{
		engCounter = 1;
		wasOverflow++;
	}
	//CalculatePosition();
}

inline void ledON()
{
	LED_PORT |= 1 << CONT_LED;
}
inline void ledOFF()
{
	LED_PORT &= ~(1 << CONT_LED);
}

inline void ledToggle()
{
	LED_PORT ^= (1 << CONT_LED);
}

void SetEngines()
{
	if(wasInterrupt)
	{
		wasInterrupt = 0;
		if(engCounter % 10 == 0)
		{
			if(aLEngineFill < lEngineFill)
				aLEngineFill++;
			if(aREngineFill < rEngineFill)
				aREngineFill ++;
			if(aLEngineFill > lEngineFill)
				aLEngineFill--;
			if(aREngineFill > rEngineFill)
				aREngineFill --;
		}

		if(engCounter <= abs(aREngineFill))
		{
			if(aREngineFill > 0)
				engine_Eng1DriveForward();
			else if(aREngineFill < 0)
				engine_Eng1DriveBack();

		}
		else
			engine_Eng1Stop();

		if(engCounter <= abs(aLEngineFill))
		{
			if(aLEngineFill > 0)
				engine_Eng2DriveForward();
			else if(aLEngineFill < 0)
				engine_Eng2DriveBack();
		}
		else
			engine_Eng2Stop();
	}
}

void SetRadioStateReset()
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
			radioState = RESET;
		}
	}
}

int IsTimeForAction()
{
	return irCounter == radioActionTimer ||
			irCounter == (radioActionTimer + 1) ||
			irCounter == (radioActionTimer + 2);
}

void SetNextActionTime(int delayMS)
{
	radioActionTimer = irCounter + delayMS;
}

///Function manages state of the radio transceiver,
///sends and receives data.
void RadioReceive()
{
	switch(radioState)
	{
		case REC:
		{
			if(radio_IsInterruptRequest())
			{
				//ledToggle();
				radioReceived = 1;
				radio_StopListenning();
				while(!radio_IsReceivingBuforEmpty())
				{
					radio_Receive(radioRecBufor, D_WIDTH);
				}
				if(ProcessDataFromRadio(radioRecBufor)) //if transmitter wants respond
				{
					radio_SwitchTransmiter();
					radioState = WFBT;
					SetNextActionTime(28);
				}
				else
				{
					radio_StartListenning();
				}
			}
			break;
		}
		case WFBT:
		{
			if(IsTimeForAction())
			{
				radioState = TRA1;
			}
			break;
		}
		case TRA1:
		{
			radio_PreparePayload(radioSendBufor, D_WIDTH);
			SetNextActionTime(10);
			radioState = TRA2;
			break;
		}
		case TRA2:
		{
			if(IsTimeForAction())
			{
				radio_Transmit();
				SetNextActionTime(10);
				radioState = WFTR;
			}
			break;
		}
		case WFTR:
		{
			if(radio_IsInterruptRequest() || IsTimeForAction())
			{
				if(radio_IsInterruptRequest())
					//ledToggle();
				radio_Reset();
				radio_SwitchReceiver();
				SetNextActionTime(10);
				radioState = WFBR;
			}
			break;
		}
		case WFBR:
		{
			if(IsTimeForAction())
			{
				radioState = REC;
				radio_StartListenning();
			}
			break;
		}
		case RESET:
		{
			lEngineFill = rEngineFill = 0;
			isFollowingPoint = 0, isFollowingLine = 0;
			radio_Reset();
			radio_Init(1, 1, D_WIDTH);//receiver, 5b data
			SetNextActionTime(20);
			radioState = WFBR;
			break;
		}
	}
}


void SendInfo()
{
	uart_sendString("\tReceived!");
	uart_sendByteAsChar(radioRecBufor[0]);
}

///Function returns 1 if transmitter wants respond
///and 0 if it doesn't.
///If transmitter wants respond it also prepares the radioSendBuffor
///with proper data payload. If received packet is valid function
///fill connected variables like engine's fills or goal positions
///with data received in packet.
uint8_t ProcessDataFromRadio(uint8_t* data)
{
	switch(data[0])
	{
		case GETPOS:
		{
			radioSendBufor[0] = GETPOS;
			getBytes(posX, radioSendBufor + 1);
			getBytes(posY, radioSendBufor + 5);
			getBytes(angle, radioSendBufor + 9);
			radioSendBufor[13] = round(abs(aLEngineFill) * 100 / PERIOD);
			radioSendBufor[14] = round(abs(aREngineFill) * 100 / PERIOD);
			return 1;
		}
		case GETBA:
		{
			radioSendBufor[0] = GETBA;
			getBytes(wheelSize, radioSendBufor + 1);
			getBytes(wheelSpacing, radioSendBufor + 5);
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
			int percentageLEF = (data[1] & 0b01111111);
			int percentageREF = (data[2] & 0b01111111);
			lEngineFill = percentageLEF * PERIOD / 100;
			rEngineFill = percentageREF * PERIOD / 100;
			if(lEngineFill)
			{
				if(!(data[1] & 0b10000000))
					lEngineFill = - lEngineFill;
			}
			if(rEngineFill)
			{
				if(!(data[2] & 0b10000000))
					rEngineFill = -rEngineFill;
			}
			return 0;
		}
		case SETPO:
		{
			posX = getValFromBytes(data + 1);
			posY = getValFromBytes(data + 5);
			angle = getValFromBytes(data + 9);
			return 0;
		}
		case SETBA:
		{
			wheelSize = getValFromBytes(data + 1);
			wheelSpacing = getValFromBytes(data + 5);
			wheelPerimeter = wheelSize * M_PI;
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

