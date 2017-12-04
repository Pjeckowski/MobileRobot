/*
 * my_uart.h
 *
 *  Created on: 9 maj 2017
 *      Author: Lapek
 */

#ifndef MY_UART_H_
#define MY_UART_H_



#include<avr/io.h>
#include<avr/interrupt.h>
#include<util/delay.h>
#include<avr/pgmspace.h>
#include<string.h>

void uart_init()
{
	uint16_t baud=51;
	// sets baudrate to 9600 bps
	UBRRH = (baud >> 8);
	UBRRL = baud;

	UCSRC |= (1 << URSEL)|(1 << UCSZ0)|(1 << UCSZ1)|(1 << USBS)|(1 << UPM1); //8bit data frame, 2 stop bits, even parity

	UCSRB |= (1 << TXEN)|(1 << RXEN)|(1 << RXCIE); //enable transmitter, receiver, and data received interrupt

}

void uart_send(uint8_t data)
{
    while (!( UCSRA & (1<<UDRE))); // wait while register is free
    UDR = data;                    // load data in the register
}

uint8_t uart_receive()
{
	while(!((UCSRA) & (1<<RXC)));     // wait while data is being received
	return UDR;                     // return data
}

void uart_sendByteAsChar(uint8_t data)
{
	int i;
	for(i = 0; i < 8; i++)
	{
		if((data >> (7-i)) & 1)
			uart_send('1');
		else
			uart_send('0');
	}
	uart_send('\r');

}

void uart_sendString(char* table)
{
	int i;
	for(i = 0; i < strlen(table); i++)
	{
		uart_send(table[i]);
	}
	uart_send('\r');
}

void uart_sendValueAsChar(uint8_t data)
{
	if(data == 0)
	{
		uart_send('0');
	}
	else
	{
		uint8_t dec = 10;
		while(dec < data)
			dec*=10;
		dec/=10;
		while(0 != data)
		{
			uart_send(data/dec + 48);
			data = data % dec;
			dec /= 10;
		}
	}
}

#endif /* MY_UART_H_ */
