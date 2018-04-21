/*
 * adc.h
 *
 *  Created on: 13 kwi 2018
 *      Author: Patryk
 */

#ifndef ADC_H_
#define ADC_H_

#include<avr/io.h>

#define ADC_IDLE 0
#define ADC_STARTED 1
#define REF_VCC 	(1 << REFS0)
#define REF_256 	(1 << REFS0) | (1 << REFS1)
#define ADC_P2		0
#define ADC_P4		(1 << ADPS1)
#define ADC_P8		(1 << ADPS0) | (1 << ADPS1)
#define ADC_P16		(1 << ADPS2)
#define ADC_P32		(1 << ADPS0) | (1 << ADPS2)
#define ADC_P64		(1 << ADPS1) | (1 << ADPS2)
#define ADC_P128	(1 << REFS0) | (1 << ADPS1) | (1 << ADPS2)

void adc_Init(uint8_t ref, uint8_t prescaller);
void adc_StartConversion(uint8_t input);
int adc_IsConversionDone();

#endif /* ADC_H_ */
