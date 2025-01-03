
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
        public static async Task WriteServoData(string message, Pca9685 pca)
        {
            // Check if the task is already running
            if (isTaskRunning)
            {
               
                return;
            }

            try
            {
                // Mark the task as running
                isTaskRunning = true;

                var unpackedState = ControllerStateParser.UnpackState(message);

                SetServoAngle(pca, 0, unpackedState.LeftThumbY);
                SetServoAngle(pca, 1, unpackedState.RightThumbX);
                SetServoAngle(pca, 2, unpackedState.RightThumbY);


            }
            finally
            {
                // Mark the task as not running when it completes
                isTaskRunning = false;
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

        private static class ControllerStateParser
        {
            public static (int LeftThumbY, int LeftThumbX, int RightThumbX, int RightThumbY, bool YButton, bool BButton) UnpackState(string message)
            {
                // Ensure the message has the expected length and structure
                if (message.Length < 10)
                {
                    throw new ArgumentException("Invalid message format.");
                }

                // Parse joystick values from the string
                int leftThumbY = int.Parse(message.Substring(1, message.IndexOf('w') - 1)); // Extract value before 'w'
                int leftThumbX = int.Parse(message.Substring(message.IndexOf('w') + 1, message.IndexOf('e') - message.IndexOf('w') - 1));
                int rightThumbX = int.Parse(message.Substring(message.IndexOf('e') + 1, message.IndexOf('r') - message.IndexOf('e') - 1));
                int rightThumbY = int.Parse(message.Substring(message.IndexOf('r') + 1, message.IndexOf('T') - message.IndexOf('r') - 1));

                // Parse button states (T or F) for Y and B buttons
                bool yButtonPressed = message.EndsWith("T") && message[message.IndexOf('T')] == 'T';
                bool bButtonPressed = message[message.IndexOf('T') + 1] == 'T';

                // Return unpacked data
                return (leftThumbY, leftThumbX, rightThumbX, rightThumbY, yButtonPressed, bButtonPressed);
            }
        }
    }
}
