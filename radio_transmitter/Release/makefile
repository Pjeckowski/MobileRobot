################################################################################
# Automatically-generated file. Do not edit!
################################################################################

-include ../makefile.init

RM := rm -rf

# All of the sources participating in the build are defined here
-include sources.mk
-include subdir.mk
-include objects.mk

ifneq ($(MAKECMDGOALS),clean)
ifneq ($(strip $(ASM_DEPS)),)
-include $(ASM_DEPS)
endif
ifneq ($(strip $(S_DEPS)),)
-include $(S_DEPS)
endif
ifneq ($(strip $(S_UPPER_DEPS)),)
-include $(S_UPPER_DEPS)
endif
ifneq ($(strip $(C_DEPS)),)
-include $(C_DEPS)
endif
endif

-include ../makefile.defs

# Add inputs and outputs from these tool invocations to the build variables 
LSS += \
radio_transmitter.lss \

FLASH_IMAGE += \
radio_transmitter.hex \

EEPROM_IMAGE += \
radio_transmitter.eep \

SIZEDUMMY += \
sizedummy \


# All Target
all: radio_transmitter.elf secondary-outputs

# Tool invocations
radio_transmitter.elf: $(OBJS) $(USER_OBJS)
	@echo 'Building target: $@'
	@echo 'Invoking: AVR C Linker'
	avr-gcc -Wl,-Map,radio_transmitter.map -mmcu=atmega8 -o "radio_transmitter.elf" $(OBJS) $(USER_OBJS) $(LIBS)
	@echo 'Finished building target: $@'
	@echo ' '

radio_transmitter.lss: radio_transmitter.elf
	@echo 'Invoking: AVR Create Extended Listing'
	-avr-objdump -h -S radio_transmitter.elf  >"radio_transmitter.lss"
	@echo 'Finished building: $@'
	@echo ' '

radio_transmitter.hex: radio_transmitter.elf
	@echo 'Create Flash image (ihex format)'
	-avr-objcopy -R .eeprom -R .fuse -R .lock -R .signature -O ihex radio_transmitter.elf  "radio_transmitter.hex"
	@echo 'Finished building: $@'
	@echo ' '

radio_transmitter.eep: radio_transmitter.elf
	@echo 'Create eeprom image (ihex format)'
	-avr-objcopy -j .eeprom --no-change-warnings --change-section-lma .eeprom=0 -O ihex radio_transmitter.elf  "radio_transmitter.eep"
	@echo 'Finished building: $@'
	@echo ' '

sizedummy: radio_transmitter.elf
	@echo 'Invoking: Print Size'
	-avr-size --format=avr --mcu=atmega8 radio_transmitter.elf
	@echo 'Finished building: $@'
	@echo ' '

# Other Targets
clean:
	-$(RM) $(FLASH_IMAGE)$(ELFS)$(OBJS)$(ASM_DEPS)$(EEPROM_IMAGE)$(S_DEPS)$(SIZEDUMMY)$(S_UPPER_DEPS)$(LSS)$(C_DEPS) radio_transmitter.elf
	-@echo ' '

secondary-outputs: $(LSS) $(FLASH_IMAGE) $(EEPROM_IMAGE) $(SIZEDUMMY)

.PHONY: all clean dependents
.SECONDARY:

-include ../makefile.targets
