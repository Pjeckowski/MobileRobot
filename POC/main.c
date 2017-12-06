/*
 * main.c
 *
 *  Created on: 6 gru 2017
 *      Author: Patryk
 */


#include<avr/io.h>
#include<util/delay.h>
#include "uart\my_uart.h"

void getBytes(double data, uint8_t* bytes);
double getValFromBytes(uint8_t bytes[4]);

int main()
{
	_delay_ms(1000);

	uart_init();
	double a = 0;
	double b = 0;
	uint8_t bytes[4];
	int c = 0;
	_delay_ms(1000);

	while(1)
	{
		a++;
		_delay_ms(1000);
		getBytes(a, bytes);
		b = getValFromBytes(bytes);
		c = (int) b;
		uart_sendValueAsChar(c);
	}
}

void getBytes(double data, uint8_t* bytes)
{
	memcpy(bytes, (uint8_t *)(&data), 4);
}

double getValFromBytes(uint8_t bytes[4])
{
	double val = 0;
	memcpy((uint8_t*)(&val), bytes, 4);
	return val;
}
