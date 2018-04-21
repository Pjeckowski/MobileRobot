/*
 * rfSensor.c
 *
 *  Created on: 18 kwi 2018
 *      Author: Patryk
 */

#include"rfSensor.h"
#include"../adc/adc.h"


ReflectiveSensor refSensor_Init(uint8_t leftPinNumber, uint8_t middlePinNumber, uint8_t rightPinNumber, uint16_t edge)
{
    ReflectiveSensor rS =
    {
        .LeftIn = leftPinNumber,
        .MiddleIn = middlePinNumber,
        .RightIn = rightPinNumber,
        .refSensorStatus = _RSCIDLE,
        .conValue = -1,
        .rsValues = {.LeftIn = 0, .MiddleIn = 0, .RightIn = 0},
        .isReady = 0,
        ._1edge = edge
    };
    adc_Init(REF_VCC, ADC_P16);
    return rS;
}

uint8_t refSensor_AreValuesReady(ReflectiveSensor *rS)
{
    if(rS->isReady)
        return 1;
	switch(rS->refSensorStatus)
	{
		case _RSCIDLE:
		{
			adc_StartConversion(rS->LeftIn);
			rS->refSensorStatus = _RSC0;
			return 0;
		}
		case _RSC0:
		{
			rS->conValue = adc_IsConversionDone();
			if(-1 != rS->conValue)
			{
				if(rS->conValue > rS->_1edge)
					rS->rsValues.LeftIn = 1;
				else
					rS->rsValues.LeftIn = 0;

				adc_StartConversion(rS->MiddleIn);
				rS->refSensorStatus = _RSC1;
			}
			return 0;
		}
		case _RSC1:
		{
			rS->conValue = adc_IsConversionDone();
			if(-1 != rS->conValue)
			{
				if(rS->conValue > rS->_1edge)
					rS->rsValues.MiddleIn = 1;
				else
					rS->rsValues.MiddleIn = 0;

				adc_StartConversion(rS->RightIn);
				rS->refSensorStatus = _RSC2;
			}
			return 0;
		}
		case _RSC2:
		{
			rS->conValue = adc_IsConversionDone();
			if(-1 != rS->conValue)
			{
				if(rS->conValue > rS->_1edge)
					rS->rsValues.RightIn = 1;
				else
					rS->rsValues.RightIn = 0;

				adc_StartConversion(rS->LeftIn);
				rS->refSensorStatus = _RSCIDLE;
			}
			rS->isReady = 1;
			return 1;
		}
		default:
		{
			return 0;
		}
	}
}

void refSensor_Clean(ReflectiveSensor *rS)
{
	rS->isReady = 0;
}
