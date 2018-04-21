/*
 * adc.c
 *
 *  Created on: 13 kwi 2018
 *      Author: Patryk
 */

#include"adc.h"

static volatile uint8_t adcStatus = 0;

void adc_Init(uint8_t ref, uint8_t prescaller)
{
	ADCSRA |= (1 << ADEN);
	ADCSRA |= prescaller;
	ADMUX |= ref;
	adcStatus = ADC_IDLE;
}

void adc_StartConversion(uint8_t input)
{
	ADMUX = (ADMUX & 0b11111000) | input;
	ADCSRA |= (1 << ADSC);
	adcStatus = ADC_STARTED;
}

int adc_IsConversionDone()
{
	if(ADC_STARTED != adcStatus)
		return -1;
	if(ADCSRA & (1 << ADSC))
		return -1;
	adcStatus = ADC_IDLE;
	return (int)ADCW;
}
