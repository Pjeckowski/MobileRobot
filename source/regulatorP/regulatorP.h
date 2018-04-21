/*
 * regulatorP.h
 *
 *  Created on: 21 kwi 2018
 *      Author: Patryk
 */

#ifndef REGULATORP_REGULATORP_H_
#define REGULATORP_REGULATORP_H_

#include<avr/io.h>

typedef struct RegulatorParams
{
	float Nearest;
	float Medium;
	float Farthest;
	float KP;
	int TP;
	int MaxValue;
}RegulatorParams;

typedef struct RegulatorWheelControl
{
	int LeftWheel;
	int RightWheel;
}RegulatorWheelControl;

RegulatorParams regulator_Init(float nearest, float medium,
		float farthest, float kp, int tp, int maxValue);

float regulator_GetError(uint8_t lF, uint8_t lM, uint8_t lN, uint8_t pN,
		uint8_t pM, uint8_t pF, RegulatorParams rP);

RegulatorWheelControl regulator_GetControl(float error, RegulatorParams rP);

#endif /* REGULATORP_REGULATORP_H_ */
