using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.System.Threading;

namespace BackgroundApplication2
{

    internal class IotController
    {
        private const int SERVO_MOTOR_PIN = 18;
        public static GpioPin pin;
        public static GpioController gpio;

        internal void ProcessRemoteOrder(string path)
        {
            var open = path == "/ON";

            gpio = GpioController.GetDefault();
            try
            {
                TryInitGPIO(open);
                MoveServoMotor(!open);
            }
            catch (Exception)
            {
            }
        }

        private void MoveServoMotor(bool isClockWise)
        {
            var stopwatch = Stopwatch.StartNew();
            var workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                 (source) =>
                 {
                     // setup, ensure pins initialized
                     ManualResetEvent mre = new ManualResetEvent(false);
                     mre.WaitOne(1500);

                     ulong pulseTicks = ((ulong)(Stopwatch.Frequency) / 1000) * 2;
                     ulong delta;
                     var startTime = stopwatch.ElapsedMilliseconds;
                     while (stopwatch.ElapsedMilliseconds - startTime <= 300)
                     {
                         pin.Write(GpioPinValue.High);
                         ulong starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = isClockWise ? (ulong)(stopwatch.ElapsedTicks) - starttick : starttick - (ulong)(stopwatch.ElapsedTicks);
                             if (delta > pulseTicks) break;
                         }
                         pin.Write(GpioPinValue.Low);
                         starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                             if (delta > pulseTicks * 10) break;
                         }
                     }
                 }, WorkItemPriority.High);
        }

        private void TryInitGPIO(bool open)
        {
            try
            {
                if (gpio == null)
                {
                    pin = null;
                    return;
                }
                pin = gpio.OpenPin(SERVO_MOTOR_PIN);
                pin.Write(GpioPinValue.High);
                pin.SetDriveMode(GpioPinDriveMode.Output);
            }
            catch (Exception)
            {
            }
        }
        
    }
}