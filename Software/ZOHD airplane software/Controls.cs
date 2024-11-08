
using System;


using System.Runtime.CompilerServices;

namespace UDPtest 
{
    using System;

  
    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Abstractions;
    using Unosquare.RaspberryIO;
    using System.Runtime.InteropServices;
  

    public static class Controls
    {
        // Define GPIO pins for three servos
        const uint ServoPin1 = 5; // GPIO pin for servo 1
        const uint ServoPin2 = 6; // GPIO pin for servo 2
        const uint ServoPin3 = 13; // GPIO pin for servo 3
        private static bool _isUpdating = false;
        // Public static integers for desired angles of each servo
        public static int DesiredAngle1 = 90; // Angle for servo 1
        public static int DesiredAngle2 = 45; // Angle for servo 2
        public static int DesiredAngle3 = 135; // Angle for servo 3

        // DLL imports for pigpio functions
        [DllImport("libpigpio.so", EntryPoint = "gpioInitialise")]
        private static extern int GpioInitialise();

        [DllImport("libpigpio.so", EntryPoint = "gpioSetMode")]
        private static extern int GpioSetMode(uint gpio, uint mode);

        [DllImport("libpigpio.so", EntryPoint = "gpioServo")]
        private static extern int GpioServo(uint gpio, uint pulsewidth);

        [DllImport("libpigpio.so", EntryPoint = "gpioGetServoPulsewidth")]
        private static extern int GpioGetServoPulsewidth(uint gpio);

        [DllImport("libpigpio.so", EntryPoint = "gpioTerminate")]
        private static extern void GpioTerminate();

        const uint PI_OUTPUT = 1; // Define output mode

        public static void Init()
        { 
            // Initialize pigpio
            if (GpioInitialise() < 0)
            {
                Console.WriteLine("pigpio initialization failed.");
                return;
            }

            // Set the GPIO pin modes to OUTPUT
            GpioSetMode(ServoPin1, PI_OUTPUT);
            GpioSetMode(ServoPin2, PI_OUTPUT);
            GpioSetMode(ServoPin3, PI_OUTPUT);

            // Move each servo to the desired angles
            MoveServoToAngle(ServoPin1, DesiredAngle1);
            MoveServoToAngle(ServoPin2, DesiredAngle2);
            MoveServoToAngle(ServoPin3, DesiredAngle3);

            // Clean up and stop pigpio
            GpioTerminate();
        }

        public static async Task UpdateServosAsync(string messsage)
        {
            // Check if the servos are already updating
            if (_isUpdating)
            {
                Console.WriteLine("Servos are already updating. Exiting the method.");
                return; // Exit if already updating
            }

            _isUpdating = true; // Set the flag to indicate servos are updating
            try
            {
                ParseOutput(messsage);
                 MoveServoToAngle(ServoPin1, DesiredAngle1);
                 MoveServoToAngle(ServoPin2, DesiredAngle2);
                 MoveServoToAngle(ServoPin3, DesiredAngle3);
            }
            finally
            {
                _isUpdating = false; // Reset the flag when done
            }
        }

        // Function to move the servo to the desired angle
        public static void MoveServoToAngle(uint servoPin, int angle)
        {
            if (angle < 0 || angle > 180)
            {
                Console.WriteLine($"Angle out of range for pin {servoPin}. Please use a value between 0 and 180.");
                return;
            }

            uint pulseWidth = (uint)MapAngleToPulseWidth(angle); // Cast to uint
            GpioServo(servoPin, pulseWidth);
           
           
        }

        // Function to map angle to pulse width
        private static int MapAngleToPulseWidth(int angle)
        {
            return 530 + (angle * (2370 - 530) / 180); // Map 0-180 to 530-2370 us
        }

        public static void ParseOutput(string output)
        {
            if (output.Length < 5) // Ensure the string is long enough
                throw new ArgumentException("Output string is too short.");

            // Extract servo angles
            DesiredAngle1 = Int32.Parse(output.Substring(0, output.IndexOf('q')));
            DesiredAngle2 = Int32.Parse(output.Substring(output.IndexOf('q') + 1, output.IndexOf('w') - output.IndexOf('q') - 1));
            DesiredAngle3 = Int32.Parse(output.Substring(output.IndexOf('w') + 1, output.IndexOf('e') - output.IndexOf('w') - 1));

            // Extract button states
          //  ButtonYPressed = output[^2] == 'T'; // Last second last character
           // ButtonBPressed = output[^1] == 'T'; // Last character
        }

      
    }
}

