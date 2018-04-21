################################################################################
# Automatically-generated file. Do not edit!
################################################################################

-include ../makefile.init

RM := rm -rf

# All of the sources participating in the build are defined here
-include sources.mk
-include source/rfSensor/subdir.mk
-include source/regulatorP/subdir.mk
-include source/adc/subdir.mk
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
mobileRobot.lss \

FLASH_IMAGE += \
mobileRobot.hex \

EEPROM_IMAGE += \
mobileRobot.eep \

SIZEDUMMY += \
sizedummy \


# All Target
all: mobileRobot.elf secondary-outputs

# Tool invocations
mobileRobot.elf: $(OBJS) $(USER_OBJS)
	@echo 'Building target: $@'
	@echo 'Invoking: AVR C Linker'
	avr-gcc -Wl,-Map,mobileRobot.map -mmcu=atmega32 -o "mobileRobot.elf" $(OBJS) $(USER_OBJS) $(LIBS)
	@echo 'Finished building target: $@'
	@echo ' '

mobileRobot.lss: mobileRobot.elf
	@echo 'Invoking: AVR Create Extended Listing'
	-avr-objdump -h -S mobileRobot.elf  >"mobileRobot.lss"
	@echo 'Finished building: $@'
	@echo ' '

mobileRobot.hex: mobileRobot.elf
	@echo 'Create Flash image (ihex format)'
	-avr-objcopy -R .eeprom -R .fuse -R .lock -R .signature -O ihex mobileRobot.elf  "mobileRobot.hex"
	@echo 'Finished building: $@'
	@echo ' '

mobileRobot.eep: mobileRobot.elf
	@echo 'Create eeprom image (ihex format)'
	-avr-objcopy -j .eeprom --no-change-warnings --change-section-lma .eeprom=0 -O ihex mobileRobot.elf  "mobileRobot.eep"
	@echo 'Finished building: $@'
	@echo ' '

sizedummy: mobileRobot.elf
	@echo 'Invoking: Print Size'
	-avr-size --format=avr --mcu=atmega32 mobileRobot.elf
	@echo 'Finished building: $@'
	@echo ' '

# Other Targets
clean:
	-$(RM) $(FLASH_IMAGE)$(ELFS)$(OBJS)$(ASM_DEPS)$(EEPROM_IMAGE)$(S_DEPS)$(SIZEDUMMY)$(S_UPPER_DEPS)$(LSS)$(C_DEPS) mobileRobot.elf
	-@echo ' '

secondary-outputs: $(LSS) $(FLASH_IMAGE) $(EEPROM_IMAGE) $(SIZEDUMMY)

.PHONY: all clean dependents
.SECONDARY:

-include ../makefile.targets
