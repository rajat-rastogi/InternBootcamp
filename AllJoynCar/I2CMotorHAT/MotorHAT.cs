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
    //Stepper motor is not present as it is untested. 
    //To add stepper motor follow same logic as DC motor.

    public sealed class MotorHAT
    {
        internal PWM pwm;
        private DCMotor[] motors;

        public MotorHAT()
        {           
            motors = new DCMotor[] { new DCMotor(this, 0), new DCMotor(this, 1), new DCMotor(this, 2), new DCMotor(this, 3) };
            pwm = new PWM();
        }

        internal void SetPin(uint pin, bool value)
        {
            if (pin > 15)
            {
                throw new ArgumentException("Invalid pin.", "pin");
            }                 

            if (value)
                pwm.SetPWMRegister(pin, 4096, 0);
            else
                pwm.SetPWMRegister(pin, 0, 4096);
        }

        public DCMotor GetMotor(uint motorNumber)
        {
            if (motorNumber < 1 || motorNumber > 4)
            {
                throw new ArgumentException("Invalid motor number.", "motorNumber");
            }

            return motors[motorNumber - 1];

        }
    }
}
