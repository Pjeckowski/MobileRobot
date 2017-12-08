#ifndef ENGINE_H_
#define ENGINE_H_

//requests

#define GETX	0b10000001
#define GETY	0b10000010
#define GETA	0b10000011


//commands

#define SETGX	0b00000001
#define SETGY 	0b00000010
#define SETEF	0b00000011
#define FOTOP	0b00000100
#define FOLIN	0b00000101
#define SETME	0b00000110
#define RSTOP	0b01111111

//other
#define ERROR	0b01010101

#endif /* ENGINE_H_ */
