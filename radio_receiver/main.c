/*
 * radio_rec.c
 *
 *  Created on: 8 mar 2017
 *      Author: Patryk
 */

#include<avr/io.h>
#include<util/delay.h>
#include "nrf24l01\rf24l01.h"
#include "uart\my_uart.h"
#include "robot\engine.h"
#include <String.h>

#define Control_led PD6
#define IRQ 	PA0
#define IRQPIN	PINA
#define time1ms 255 -31

uint8_t data[5];
uint8_t rcv_data[32];
char *table = "a";
char status;

void radioRec();
void dataWorkout();
uint8_t* getTransmitData(uint8_t request);

volatile uint8_t irCounter;
enum nrfState {REC, WFBT, WFBR, TRA1, TRA2, WFTR, WFRE};
enum nrfState RadioState = REC;
uint8_t radio_actionTimer = 0;

int main()
{
	//PA0 jako wejscie podci¹gniête do VCC
	DDRA = 0b00000000;
	PORTA = 0b00000001;
	_delay_ms(1000);
	init_SPI(1,0); //master, fosc/128
	_delay_ms(1000);
	init_radio(1,0,0);//receiver
	_delay_ms(100);
	uart_init();

	TCCR0 |= (1 << CS02);
	TIMSK |= (1 << TOIE0);
	sei();
	_delay_ms(100);
	table = "Initialization";
	uart_sendString(table);
	uart_sendByteAsChar(get_reg(STATUS));
	_delay_ms(2000);
	PORTB |= (1 << CE);
	//radio_switchTransmiter();
	//_delay_ms(100);

	while(1)
	{
		/*data[0] += 1;
		if(data[0] == 128)
			data[0] = 0;
		radio_preparePayload(data,1);
		_delay_ms(10);
		radio_transmit();
		_delay_ms(2000);*/
		radioRec();
    }
}

ISR(TIMER0_OVF_vect)
{
	TCNT0 = time1ms;
	irCounter ++;
}

void radioRec()
{
	switch(RadioState)
	{
		case REC:
		{
			if(!(IRQPIN & (1 << IRQ)))
			{
				PORTB &= ~(1 << CE);
				while(get_reg(STATUS) != 0b00001110)
				{
					radio_receive(rcv_data, 1);
				}
				dataWorkout();

				if(rcv_data[0] & 0x80) //if transmiter wants respond
				{
					radio_switchTransmiter();
					RadioState = WFBT;
					radio_actionTimer = irCounter + 10;
				}
				else
				{
					PORTB |= (1 << CE);
				}
			}
			break;
		}
		case WFBT:
		{
			if(irCounter == radio_actionTimer)
			{
				RadioState = TRA1;
			}
			break;
		}
		case TRA1:
		{
			uart_sendString("Gonna transmit!");
			radio_preparePayload(getTransmitData(rcv_data[0]),1);
			radio_actionTimer = irCounter + 10;
			RadioState = TRA2;
			break;
		}
		case TRA2:
		{
			if(irCounter == radio_actionTimer)
			{
				radio_transmit();
				radio_actionTimer = irCounter + 10;
				RadioState = WFTR;
			}
			break;
		}
		case WFTR:
		{
			if((!(IRQPIN & (1 <<IRQ))) || irCounter == radio_actionTimer)
			{
				if(get_reg(STATUS) == 0b00100000)
				{
					table = "\tTransmit succesfull!";
					uart_sendString(table);
				}
				uart_sendString("after transmition:");
				uart_sendByteAsChar(get_reg(STATUS));
				reset_radio();
				radio_switchReceiver();
				radio_actionTimer = irCounter + 10;
				RadioState = WFBR;
			}
			break;
		}
		case WFBR:
		{
			if(irCounter == radio_actionTimer)
			{
				RadioState = REC;
				PORTB |= (1 << CE);
			}
			break;
		}
		case WFRE:
		{
			break;
		}
	}
}

void sendInfo()
{
	table = "\tReceived!";
	uart_sendString(table);
	uart_sendByteAsChar(rcv_data[0]);
}

void dataWorkout(uint8_t* data)
{
	//switch(data[0])
	//{
	//	case
	//}
}

uint8_t* getTransmitData(uint8_t request)
{
	rcv_data[0] += 1;
	return rcv_data;
}

