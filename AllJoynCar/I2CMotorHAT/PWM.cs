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
using System.Diagnostics;

namespace I2CMotorHAT
{
    class PWM
    {
        private const byte MODE1 = 0x00;
        private const byte MODE2 = 0x01;
        private const byte SUBADDR1 = 0x02;
        private const byte SUBADDR2 = 0x03;
        private const byte SUBADDR3 = 0x04;
        private const byte PRESCALE = 0xFE;
        private const byte LED0_ON_L = 0x06;
        private const byte LED0_ON_H = 0x07;
        private const byte LED0_OFF_L = 0x08;
        private const byte LED0_OFF_H = 0x09;
        private const byte ALL_LED_ON_L = 0xFA;
        private const byte ALL_LED_ON_H = 0xFB;
        private const byte ALL_LED_OFF_L = 0xFC;
        private const byte ALL_LED_OFF_H = 0xFD;

        private const byte RESTART = 0x80;
        private const byte SLEEP = 0x10;
        private const byte ALLCALL = 0x01;
        private const byte INVRT = 0x10;
        private const byte OUTDRV = 0x04;

        private float prescalevalue;
        private float prescale;

        private I2CHandler i2cHandler;        

        public PWM()
        {
            i2cHandler = new I2CHandler();
            InitPWM();
        }               

        private void InitPWM()
        {
            SetAllPWMRegisters(0, 0);

            try
            {
                i2cHandler.WriteToI2C(new byte[] { MODE2, OUTDRV });
                i2cHandler.WriteToI2C(new byte[] { MODE1, ALLCALL });

            }
            catch (System.IO.IOException e)
            {
                System.Diagnostics.Debug.WriteLine("Could not write to registers: " + e.Message);
                throw e;
            }
        }
     
        internal void SetPWMFrequency(float frequency)
        {
            prescalevalue = 25000000; //25MHz
            prescalevalue /= 4096;    //12-bit
            prescalevalue /= frequency;
            prescalevalue -= 1;

            Debug.WriteLine("Setting PWM frequency to " + frequency + " Hz");
            Debug.WriteLine("Estimated pre-scale: " + prescalevalue);

            prescale = (float)Math.Floor(prescalevalue + 0.5);

            if (prescale > 255)
            {
                throw new ArgumentException("Frequency is too small.", "frequency");                
            }

            Debug.WriteLine("Final pre-scale: " + prescale);
            byte[] registerAddressBuffer = new byte[] { MODE1 };
            byte[] readBuffer = new byte[1];

            i2cHandler.WriteReadToI2C(registerAddressBuffer, readBuffer);
            byte[] newMode = new byte[1];
            newMode[0] = Convert.ToByte(readBuffer[0] & Convert.ToByte(0x7F) | Convert.ToByte(0x10));
            i2cHandler.WriteToI2C(new byte[] { MODE1, newMode[0] });
            i2cHandler.WriteToI2C(new byte[] { Convert.ToByte(prescale), Convert.ToByte(Math.Floor(prescale)) });
            i2cHandler.WriteToI2C(new byte[] { MODE1, readBuffer[0] });
            i2cHandler.WriteToI2C(new byte[] { MODE1, Convert.ToByte(readBuffer[0] | 0x80) });
        }

        /// <summary>
        /// Set all the PWM registers
        /// </summary>
        /// <param name="on"></param>
        /// <param name="off"></param>
        internal void SetAllPWMRegisters(byte on, byte off)
        {
            i2cHandler.WriteToI2C(new byte[] { ALL_LED_ON_L, Convert.ToByte(on & 0xFF) });
            i2cHandler.WriteToI2C(new byte[] { ALL_LED_ON_H, Convert.ToByte(on >> 8) });
            i2cHandler.WriteToI2C(new byte[] { ALL_LED_OFF_L, Convert.ToByte(off & 0xFF) });
            i2cHandler.WriteToI2C(new byte[] { ALL_LED_OFF_H, Convert.ToByte(off >> 8) });
        }

        /// <summary>
        /// Set value of specific registers
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="on"></param>
        /// <param name="off"></param>
        internal void SetPWMRegister(uint channel, int on, int off)
        {         
            i2cHandler.WriteToI2C(new byte[] { Convert.ToByte(LED0_ON_L + 4 * channel), Convert.ToByte(on & 0xFF) });
            i2cHandler.WriteToI2C(new byte[] { Convert.ToByte(LED0_ON_H + 4 * channel), Convert.ToByte(on >> 8) });
            i2cHandler.WriteToI2C(new byte[] { Convert.ToByte(LED0_OFF_L + 4 * channel), Convert.ToByte(off & 0xFF) });
            i2cHandler.WriteToI2C(new byte[] { Convert.ToByte(LED0_OFF_H + 4 * channel), Convert.ToByte(off >> 8) });
        }
    }
}
