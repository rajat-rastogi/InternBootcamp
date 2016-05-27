//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I2CMotorHAT
{
    class StepperMotor
    {
        const uint MICROSTEPS = 8;
        readonly int[] MICROSTEP_CURVE = { 0, 50, 98, 142, 180, 212, 236, 250, 255 };

        uint PWMA;
        uint AIN2;
        uint AIN1;
        uint PWMB;
        uint BIN2;
        uint BIN1;

        readonly private MotorHAT motorHatController;
        readonly private uint stepsPerRevolution;
        private double secondsPerStep;
        private uint steppingCounter;
        private uint currentStep;
        private uint motorNumber;

        public StepperMotor(MotorHAT controller, uint number, uint steps = 200)
        {
            motorHatController = controller;
            stepsPerRevolution = steps;
            motorNumber = number;
            secondsPerStep = 0.1;
            steppingCounter = 0;
            currentStep = 0;

            number -= 1;

            if (number == 0)
            {
                PWMA = 8;
                AIN2 = 9;
                AIN1 = 10;
                PWMB = 13;
                BIN2 = 12;
                BIN1 = 11;
            }
            else if (number == 1)
            {
                PWMA = 2;
                AIN2 = 3;
                AIN1 = 4;
                PWMB = 7;
                BIN2 = 6;
                BIN1 = 5;
            }
            else
            {
                throw new ArgumentException("MotorHAT stepper must be between 1 and 2 inclusive", "number");
            }
        }

        internal void SetSpeed(uint rpm)
        {
            secondsPerStep = 60.0 / (stepsPerRevolution * rpm);
            steppingCounter = 0;
        }

        internal uint DoOneStep(Movement direction, StepStyle style)
        {
            int pwm_a, pwm_b = 255;

            if (style == StepStyle.Single)
            {
                if (((currentStep/(MICROSTEPS/2)) % 2) > 0 )
                {
                    if (direction == Movement.Forward)
                    {
                        currentStep += MICROSTEPS / 2;
                    }
                    else
                    {
                        currentStep -= MICROSTEPS / 2;
                    }
                }
                else
                {
                    if (direction == Movement.Forward)
                    {
                        currentStep += MICROSTEPS;
                    }
                    else
                    {
                        currentStep -= MICROSTEPS;
                    }
                }
            }
            
            else if (style == StepStyle.Double)
            {
                if (((currentStep / (MICROSTEPS / 2)) % 2) == 0)
                {
                    if (direction == Movement.Forward)
                    {
                        currentStep += MICROSTEPS / 2;
                    }
                    else
                    {
                        currentStep -= MICROSTEPS / 2;
                    }
                }
                else
                {
                    if (direction == Movement.Forward)
                    {
                        currentStep += MICROSTEPS;
                    }
                    else
                    {
                        currentStep -= MICROSTEPS;
                    }
                }
            }
            else if (style == StepStyle.Interleave)
            {
                if (direction == Movement.Forward)
                {
                    currentStep += MICROSTEPS / 2;
                }
                else
                {
                    currentStep -= MICROSTEPS / 2;
                }
            }

            else if (style == StepStyle.Microstep)
            {
                if (direction == Movement.Forward)
                {
                    currentStep++;
                }
                else
                {
                    currentStep--;
                }
            }

            currentStep += MICROSTEPS * 4;
            currentStep %= MICROSTEPS * 4;

            pwm_a = pwm_b = 0; 

            if ((currentStep >= 0) && (currentStep < MICROSTEPS))
            {
                pwm_a = MICROSTEP_CURVE[MICROSTEPS - currentStep];
                pwm_b = MICROSTEP_CURVE[currentStep];
            }
            else if ((currentStep >= MICROSTEPS) && (currentStep < MICROSTEPS * 2))
            {
                pwm_a = MICROSTEP_CURVE[currentStep - MICROSTEPS];
                pwm_b = MICROSTEP_CURVE[MICROSTEPS * 2 - currentStep];
            }
            else if ((currentStep >= MICROSTEPS * 2) && (currentStep < MICROSTEPS * 3))
            {
                pwm_a = MICROSTEP_CURVE[MICROSTEPS * 3 - currentStep];
                pwm_b = MICROSTEP_CURVE[currentStep - MICROSTEPS * 2];
            }
            else if ((currentStep >= MICROSTEPS * 3) && (currentStep < MICROSTEPS * 4))
            {
                pwm_a = MICROSTEP_CURVE[currentStep - MICROSTEPS * 3];
                pwm_b = MICROSTEP_CURVE[MICROSTEPS * 4 - currentStep];
            }

            currentStep += MICROSTEPS * 4;
            currentStep %= MICROSTEPS * 4;

            motorHatController.pwm.SetPWMRegister(PWMA, 0, pwm_a * 16);
            motorHatController.pwm.SetPWMRegister(PWMB, 0, pwm_b * 16);

            return currentStep;
        }

        internal void Step (uint steps, Movement direction, StepStyle step)
        {
            double secondsPerStep = this.secondsPerStep;
            uint latestStep = 0;

            if (step == StepStyle.Interleave)
            {
                secondsPerStep = secondsPerStep / 2.0;
            }
            if (step == StepStyle.Microstep)
            {
                secondsPerStep /= MICROSTEPS;
                steps *= MICROSTEPS;
            }

            System.Diagnostics.Debug.WriteLine(secondsPerStep + " sec per step");

            for (int i = 0; i < steps; ++i)
            {
                latestStep = DoOneStep(direction, step);               
            }

            if (step == StepStyle.Microstep)
            {
                while ( (latestStep != 0) && (latestStep != MICROSTEPS))
                {
                    latestStep = DoOneStep(direction, step);                    
                }
            }
        }
    }
}
