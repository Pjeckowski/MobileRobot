/*
 * main.c
 *
 *  Created on: 5 gru 2017
 *      Author: Patryk
 */
#include<avr/interrupt.h>
#include<avr/io.h>
#include<util/delay.h>
#include<uart\my_uart.h>

#define time1ms 255 -31


void setINT0risingEdgeInterrupt();
void setTIMER0ovfInterrupt();
volatile int iCounter = 1;
volatile uint8_t sendTrigger = 0;
volatile int iCounter2 = 0;
volatile int engineFill = 30;
volatile uint32_t engineCounter = 0;
volatile uint32_t back = 0;

int main()
{
	DDRA = 0b00000001;
	DDRD = 0b01100000;
	PORTB|= 0b11111111;
	setINT0risingEdgeInterrupt();
	setTIMER0ovfInterrupt();
	uart_init();

	_delay_ms(1000);
	uart_sendString("Initialization");
	_delay_ms(1000);

	sei();
	while(1)
	{
		if(sendTrigger == 1)
		{
			uart_sendString("Naliczy³:");
			uart_sendValueAsChar(engineCounter);
			engineCounter = 0;
			sendTrigger = 0;
		}
	}
}


ISR(INT0_vect)
{
	engineCounter++;
	if(PIND & (1 << PD3))
	{
		PORTA |= (1 << PA0);
		back++;
	}
	else
	{
		PORTA &= ~(1 << PA0);
	}

}

ISR(TIMER0_OVF_vect)
{

	TCNT0 = time1ms;
	if(iCounter <= engineFill)
	{
		PORTD |= (1 << PD6);
	}
	else
	{
		PORTD &= ~(1 << PD6);
	}

	iCounter ++;
	if(iCounter >= 100)
	{
		iCounter = 1;
	}

	iCounter2++;
	if(iCounter2 >= 10000)
	{
		iCounter2 = 0;
		sendTrigger = 1;
	}


}

void setINT0risingEdgeInterrupt()
{
	MCUCR |= (1 << ISC00) | (1 << ISC01);
	GICR |= (1 << INT0);
}
void setTIMER0ovfInterrupt()
{
	TCCR0 |= (1 << CS02);
	TIMSK |= (1 << TOIE0);
}

