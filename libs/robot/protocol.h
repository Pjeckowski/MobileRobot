#ifndef PROTOCOL_H_
#define PROTOCOL_H_

//requests

#define GETX	0b10000001
#define GETY	0b10000010
#define GETA	0b10000011
#define GETEF	0b10000100
#define POSU    0b00000101

#define isRequest(X) ((X) & 0x80)
//commands

#define SETGX	0b00000001
#define SETGY 	0b00000010
#define SETEF	0b00000011
#define FOTOP	0b00000100
#define FOLIN	0b00000101
#define SETME	0b00000110
#define COPAC	0b00000111
#define RSTOP	0b00111111

//other
#define ERROR	0b01010101

uint8_t headerCollection[] = 	{ GETX,
								  GETY,
								  GETA,
								  GETEF,
								  POSU,
								  SETGX,
								  SETGY,
								  SETEF,
								  FOTOP,
								  FOLIN,
								  SETME,
								  COPAC,
								  RSTOP	};

uint8_t headerCollectionLenght = sizeof(headerCollection)/sizeof(headerCollection[0]);

#endif /* ENGINE_H_ */
