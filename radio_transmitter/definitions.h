/*
 * definitions.h
 *
 *  Created on: 3 mar 2017
 *      Author: Patryk
 */

#ifndef DEFINITIONS_H_
#define DEFINITIONS_H_

#define CE 			PB3
#define CSN 		PB4
#define R 			0
#define W 			1
#define READ_REG  	0b00000000
#define WRITE_REG 	0b00100000

#define STATUS        	0x07
#define REGISTER_MASK	0x1F
#define ACTIVATE      	0x50
#define R_RX_PL_WID   	0x60
#define R_RX_PAYLOAD  	0x61
#define W_TX_PAYLOAD  	0xA0
#define W_ACK_PAYLOAD 	0xA8
#define FLUSH_TX      	0xE1
#define FLUSH_RX      	0xE2
#define REUSE_TX_PL   	0xE3
#define NOP           	0xFF
#define CONFIG			0x00
#endif /* DEFINITIONS_H_ */
