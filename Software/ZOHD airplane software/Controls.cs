
using System;
using System.Runtime.CompilerServices;

namespace UDPtest
{
    using System;
    using Iot.Device.Pwm; // For Pca9685

    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO;
    using System.Runtime.InteropServices;
    using System.Device.I2c;
    using System.Threading;
    using ZOHD_airplane_software;

    /// <summary>
    /// Control servo motors using PCA9685
    /// </summary>
    public static class Controls
    {
        /// <summary>
        /// Connects raspberry pi zero 2 w to PCA96865
        /// </summary>
        public static Pca9685 ConnectServoController()
        {
            Console.WriteLine("Connecting to PCA9685...");

            try
            {
                // Initialize I2C connection
                const int i2cBusId = 1; // Default I2C bus on Raspberry Pi
                const int pca9685Address = 0x40; // Default I2C address for PCA9685
                I2cConnectionSettings settings = new I2cConnectionSettings(i2cBusId, pca9685Address);
                I2cDevice device = I2cDevice.Create(settings);

                // Initialize PCA9685
                Pca9685 pca9685 = new Pca9685(device);
                pca9685.PwmFrequency = 50; // Set frequency to 50 Hz for servo control

                Console.WriteLine("PCA9685 Initialized.");
                I2C_on_exit_cleanUp(pca9685);

                return pca9685; // Return the PCA9685 object for use
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to PCA9685: {ex.Message}");
                return null; // Return null if connection fails
            }
        }

        private static bool isTaskRunning = false;

        /// <summary>
        /// Parses data incoming to the raspberry and writes it to servo.
        /// </summary>
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public static async Task WriteServoData(string message, Pca9685 pca)
        {
            if (!await semaphore.WaitAsync(150)) // Adjust timeout if needed
            {
                Console.WriteLine("ERROR: Servo control timed out");
                return;
            }

            try
            {
                var Instructions = UnpackMessageTools.UnpackMessages(message);
                foreach (var instruction in Instructions) 
                {
                    SetServoAngle(pca, instruction.adress, instruction.angle);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static void SetServoAngle(Pca9685 pca9685, int channel, int angle)
        {

            if (pca9685 == null)
            {
                Console.WriteLine("PCA9685 not initialized.");
                return;
            }

            // Map angle to duty cycle (5% for 0 degrees, 10% for 180 degrees)
            const double minDutyCycle = 0.05;
            const double maxDutyCycle = 0.10;
            double dutyCycle = minDutyCycle + (maxDutyCycle - minDutyCycle) * angle / 180.0;

            pca9685.SetDutyCycle(channel, dutyCycle); // Set the duty cycle for the specified channel
        }

        static readonly CancellationTokenSource _cts = new();

        static async Task I2C_on_exit_cleanUp(Pca9685 pca)
        {
            // Only attach once
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\nCtrl+C detected! Cleaning up...");
                _cts.Cancel(); // Trigger cancellation
            };

            try
            {
                await Task.Delay(Timeout.Infinite, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                // Expected when cancellation happens
            }
            finally
            {
                pca?.Dispose();
                Console.WriteLine("I²C connection closed.");
                Environment.Exit(0);
            }
        }

        

    }

 
}
