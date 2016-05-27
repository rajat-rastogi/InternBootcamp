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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Storage.Streams;

namespace I2CMotorHAT
{
    public sealed class I2CHandler
    {
        I2cDevice i2cDevice;

        private byte i2cAddress = 0x60;     //default I2C address
        private const string i2cName = "I2C1"; //RPi2 I2C name

        public I2CHandler()
        {
            if (i2cDevice != null)
                return;

            //initialize I2C communications
            try
            {
                var settings = new I2cConnectionSettings(i2cAddress);
                settings.BusSpeed = I2cBusSpeed.StandardMode;

                //find I2C device
                i2cDevice = Task.Run(async () =>
                {
                    var devicesInformation = await GetDeviceInfo();
                    var discoveredDevice = await I2cDevice.FromIdAsync(devicesInformation[0].Id, settings);

                    return discoveredDevice;
                }).Result;              
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: {0}" + e.Message);
            }

            if (i2cDevice == null)
            {
                throw new System.IO.IOException("Slave address on I2C controller is currently in use by another application.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("I2C device " + i2cDevice.DeviceId + " was found succesfully.");
            }
        }


        private static async Task<DeviceInformationCollection> GetDeviceInfo()
        {
            string aqs = I2cDevice.GetDeviceSelector(i2cName);
            var dis = await DeviceInformation.FindAllAsync(aqs);

            if (dis.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No I2C devices found.");
                return null;
            }

            return dis;
        }

        internal void WriteToI2C(byte[] data)
        {            
            i2cDevice.Write(data);
        }

        internal void WriteReadToI2C(byte[] writeBuffer, byte[] readBuffer)
        {
            i2cDevice.WriteRead(writeBuffer, readBuffer);
        }
    }
}
