
#include"engine.h"
#include<avr/io.h>


void engine_Init()
{
	//setting uC ports as outputs for direction control
	E1DIRDDR |= (1 << E1OUT1) || (1 << E1OUT2); 
	E2DIRDDR |= (1 << E2OUT1) || (1 << E2OUT2);

	//setting uC ports as inputs for encoder signal
	E1DDR &= ~(1 << E1PBPIN);
	E2DDR &= ~(1 << E2PBPIN);
	
}

inline int engine_IsEng1RotatingBackward()
{
	//checks if encoder phaseB is 1
	return E1PIN & (1 << E1PBPIN);
}

inline int engine_IsEng2RotatingBackward()
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

inline void engine_Eng1Stop()
{
	E2DIRPORT &= ~(1 << E2OUT1);
	E2DIRPORT &= ~(1 << E2OUT2);
}
