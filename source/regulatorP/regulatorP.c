/*
 * regulatorP.c
 *
 *  Created on: 21 kwi 2018
 *      Author: Patryk
 */

#include"regulatorP.h"
#include<Stdlib.h>

RegulatorParams regulator_Init(float nearest, float medium,
		float farthest, float kp, int tp, int maxValue)
{
	RegulatorParams rP =
	{
			.Nearest = nearest,
			.Medium = medium,
			.Farthest = farthest,
			.KP = kp,
			.TP = tp,
			.MaxValue = maxValue
	};
	return rP;
}

float regulator_GetError(uint8_t lF, uint8_t lM, uint8_t lN, uint8_t pN,
		uint8_t pM, uint8_t pF, RegulatorParams rP)
{
	float error = 0;
	float s = 0;
	if(lN ^ pN)
	{
		s -= lN * rP.Nearest;
		s += pN * rP.Nearest;
	}

	error = -(lM * rP.Medium + lF * rP.Farthest) + (pM * rP.Medium + pF * rP.Farthest) + s;
	return error;
}

RegulatorWheelControl regulator_GetControl(float error, RegulatorParams rP)
{
	RegulatorWheelControl rWC = {.LeftWheel = 0, .RightWheel = 0};

	rWC.LeftWheel = rP.TP + rP.KP * error;
	if(abs(rWC.LeftWheel) > rP.MaxValue)
	{
		if(rWC.LeftWheel > 0)
			rWC.LeftWheel = rP.MaxValue;
		else
			rWC.LeftWheel = -rP.MaxValue;
	}

	rWC.RightWheel = rP.TP + rP.KP * (-error);
	if(abs(rWC.RightWheel) > rP.MaxValue)
	{
		if(rWC.RightWheel > 0)
			rWC.RightWheel = rP.MaxValue;
		else
			rWC.RightWheel = -rP.MaxValue;
	}

	return rWC;
}
