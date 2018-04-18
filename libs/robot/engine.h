#ifndef ENGINE_H_
#define ENGINE_H_

#include"engine.h"
#include<avr/io.h>

//IO DEFINITIONS
// encoder inputs
#define E1DDR   DDRD
#define E1PIN 	PIND
#define E1PBPIN PD4

#define E2DDR   DDRD
#define E2PIN 	PIND
#define E2PBPIN PD5
//encoder inputs

//direction control outputs
#define E1DIRDDR  DDRC
#define E1DIRPORT PORTC
#define E1OUT1	  PC0
#define E1OUT2	  PC1

#define E2DIRDDR DDRC
#define E2DIRPORT PORTC
#define E2OUT1	  PC2
#define E2OUT2	  PC3
//direction control outputs


void engine_Init()
{
	//setting uC ports as outputs for direction control
	E1DIRDDR |= (1 << E1OUT1);
	E1DIRDDR |= (1 << E1OUT2);
	E2DIRDDR |= (1 << E2OUT1);
	E2DIRDDR |= (1 << E2OUT2);


	//setting uC ports as inputs for encoder signal
	E1DDR &= ~(1 << E1PBPIN);
	E2DDR &= ~(1 << E2PBPIN);
	PORTD |= (1 << E1PBPIN);
	PORTD |= (1 << E2PBPIN);
}

inline int engine_IsEng1RotatingBack()
{
	//checks if encoder phaseB is 1
	return E1PIN & (1 << E1PBPIN);
}

inline int engine_IsEng2RotatingBack()
{
	//checks if encoder phaseB is 1
	return E2PIN & (1 << E2PBPIN);
}
inline void engine_Eng1DriveForward()
{
	E1DIRPORT &= ~(1 << E1OUT2);
	E1DIRPORT |= (1 << E1OUT1);
}

inline void engine_Eng1DriveBack()
{
	E1DIRPORT &= ~(1 << E1OUT1);
	E1DIRPORT |= (1 << E1OUT2);
}

inline void engine_Eng1Stop()
{
	E1DIRPORT &= ~(1 << E1OUT1);
	E1DIRPORT &= ~(1 << E1OUT2);
}

inline void engine_Eng2DriveForward()
{
	E2DIRPORT &= ~(1 << E2OUT2);
	E2DIRPORT |= (1 << E2OUT1);
}

inline void engine_Eng2DriveBack()
{
	E2DIRPORT &= ~(1 << E2OUT1);
	E2DIRPORT |= (1 << E2OUT2);
}

inline void engine_Eng2Stop()
{
	E2DIRPORT &= ~(1 << E2OUT1);
	E2DIRPORT &= ~(1 << E2OUT2);
}

#endif /* ENGINE_H_ */
