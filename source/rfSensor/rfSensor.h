/*
 * rfSensor.h
 *
 *  Created on: 18 kwi 2018
 *      Author: Patryk
 */

#ifndef RFSENSOR_RFSENSOR_H_
#define RFSENSOR_RFSENSOR_H_

#include<avr/io.h>
#define _RSCIDLE 0
#define _RSC0 1 //reflective sensor conversion on input 0
#define _RSC1 2
#define _RSC2 3

typedef struct ReflectiveSensorValues
{
	uint8_t LeftIn;
	uint8_t MiddleIn;
	uint8_t RightIn;
}ReflectiveSensorValues;

typedef struct ReflectiveSensor
{
	uint8_t LeftIn;
	uint8_t MiddleIn;
	uint8_t RightIn;
	uint8_t refSensorStatus;
	int conValue;
	uint8_t leftVal;
	uint8_t rightVal;
	uint8_t midVal;
	uint8_t isReady;
	uint16_t _1edge;
}ReflectiveSensor;

ReflectiveSensor refSensor_Init(uint8_t leftPinNumber, uint8_t middlePinNumber, uint8_t rightPinNumber, uint16_t edge);
uint8_t refSensor_AreValuesReady(ReflectiveSensor *rS);
void refSensor_Clean(ReflectiveSensor *rS);




#endif /* RFSENSOR_RFSENSOR_H_ */
