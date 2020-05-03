﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureIoTHubSample1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Sample 1: Create device if you didn't have one in Azure IoT Hub, FIRST YOU NEED SPECIFY connectionString first in AzureIoTHub.cs
            //CreateDeviceIdentity();

            //Sample 2: comment above line and uncomment following line, FIRST YOU NEED SPECIFY connectingString and deviceConnectionString in AzureIoTHub.cs
            SimulateDeviceToSendD2CAndReceiveD2C();
        }

        public static void CreateDeviceIdentity()
        {
            AzureIoTHub.CreateDeviceIdentityAsync().Wait();
        }

        private static void SimulateDeviceToSendD2CAndReceiveD2C()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                tokenSource.Cancel();
                Console.WriteLine("Existing ...");
            };
            Console.WriteLine("Press CTRL+C to exit");

            Task.WaitAll(
                AzureIoTHub.SendDeviceToCloudMessageAsync(tokenSource.Token),
                AzureIoTHub.ReceiveMessagesFromDeviceAsync(tokenSource.Token)
                );
        }
    }
}
