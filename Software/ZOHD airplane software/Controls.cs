
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
             
                var unpackedState = ControllerStateParser.UnpackState(message);
                SetServoAngle(pca, 0, unpackedState.LeftThumbY);
                SetServoAngle(pca, 1, unpackedState.RightThumbX);
                SetServoAngle(pca, 2, unpackedState.RightThumbY);
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

        static async Task I2C_on_exit_cleanUp(Pca9685 pca)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; // Prevent immediate termination
                Console.WriteLine("\nCtrl+C detected! Cleaning up...");
                cts.Cancel(); // Trigger cancellation
            };

            try
            {
                // Wait until Ctrl+C is pressed (non-blocking)
                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (TaskCanceledException)
            {
                // Expected exception when cancellation token is triggered
            }
            finally
            {
                // Cleanup I²C resources
                pca?.Dispose();
                Console.WriteLine("I²C connection closed.");
                Environment.Exit(0);
            }
        }

        private static class ControllerStateParser
        {
            public static (int LeftThumbY, int LeftThumbX, int RightThumbX, int RightThumbY, bool YButton, bool BButton) UnpackState(string message)
            {
                // Declare variables
                int leftThumbY = 0, leftThumbX = 0, rightThumbX = 0, rightThumbY = 0;
                bool yButtonPressed = false, bButtonPressed = false;

                try
                {
                

                    // Ensure the message has the expected length and structure
                    if (message.Length < 10 || !message.Contains("q") || !message.Contains("w") || !message.Contains("e") || !message.Contains("r"))
                    {
                        throw new ArgumentException("Invalid message format.");
                    }

          

                    // Extract joystick values
                    leftThumbY = int.Parse(message.Substring(0, message.IndexOf('q'))); // Value before 'q'
                    leftThumbX = int.Parse(message.Substring(message.IndexOf('q') + 1, message.IndexOf('w') - message.IndexOf('q') - 1)); // Between 'q' and 'w'
                    rightThumbX = int.Parse(message.Substring(message.IndexOf('w') + 1, message.IndexOf('e') - message.IndexOf('w') - 1)); // Between 'w' and 'e'
                    rightThumbY = int.Parse(message.Substring(message.IndexOf('e') + 1, message.IndexOf('r') - message.IndexOf('e') - 1)); // Between 'e' and 'r'

         

                    // Parse button states
                    yButtonPressed = message[message.IndexOf('r') + 1] == 'T';
                    bButtonPressed = message[message.IndexOf('r') + 2] == 'T';

                
                }
                catch (Exception ex)
                {

                    Console.Clear();
                    Console.WriteLine($"Error parsing message: {ex.Message}");
                    Console.WriteLine($"Current parsed values: LeftThumbY={leftThumbY}, LeftThumbX={leftThumbX}, RightThumbX={rightThumbX}, RightThumbY={rightThumbY}, YButton={yButtonPressed}, BButton={bButtonPressed}");
                    throw;
                }

                // Return unpacked data
                return (leftThumbY, leftThumbX, rightThumbX, rightThumbY, yButtonPressed, bButtonPressed);
            }
        }
    }
}
