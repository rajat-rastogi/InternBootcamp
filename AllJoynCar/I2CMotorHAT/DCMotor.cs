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

namespace I2CMotorHAT
{
    public sealed class DCMotor
    {
        private uint motorNumber;
        private uint pwm;
        private uint input1; 
        private uint input2;

        private MotorHAT parent;

        public DCMotor(MotorHAT controller, uint number)
        {
            motorNumber = number;
            parent = controller;

            switch (number)
            {
                case 0:
                    pwm = 8;
                    input2 = 9;
                    input1 = 10;
                    break;
                case 1:
                    pwm = 13;
                    input2 = 12;
                    input1 = 11;
                    break;
                case 2:
                    pwm = 2;
                    input2 = 3;
                    input1 = 4;
                    break;
                case 3:
                    pwm = 7;
                    input2 = 6;
                    input1 = 5;
                    break;
                default:
                    throw new ArgumentException("Motor HAT only supports 4 motors", "number");
            };
        }

        ~DCMotor()
        {
            this.Run(Movement.Release);
        }
        
        public void Run(Movement command)
        {
            switch (command)
            {
                case Movement.Forward:
                    parent.SetPin(input2, false);
                    parent.SetPin(input1, true);
                    break;
                case Movement.Backward:
                    parent.SetPin(input2, true);
                    parent.SetPin(input1, false);
                    break;
                case Movement.Release:
                    parent.SetPin(input2, false);
                    parent.SetPin(input1, false);
                    break;
                case Movement.Brake:
                    throw new NotImplementedException("Motor brake command has not been implemented");
            };           
        }

        public void SetSpeed(uint speed)
        {
            if (speed > 255)
                speed = 255;

            parent.pwm.SetPWMRegister(pwm, 0, Convert.ToInt32(speed) * 16);    
        }
    }
}
