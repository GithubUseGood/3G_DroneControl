using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UDPtest 
{
    public static class Information 
    {
        private static String ReturnString;
        private static bool ErrorBool;
        private static int Voltage;
        private static GpioPin VoltagePin;
        private static int VoltagePinNum = 0;
      

        /// <summary>
        /// Initialises pin which will read battery voltage.
        /// Initialise after Controls.InitGpio
        /// </summary>
        public static void Init() 
        {
            try 
            {
               int i = Controls.Gpio.PinCount;
            } catch 
            {
                ErrorSwitch(); 
            }

            VoltagePin = Controls.Gpio.OpenPin(VoltagePinNum, PinMode.Input);
       


        }
        

        

        public static int GetSignalStrength() 
        {
            string command = "mmcli -m /org/freedesktop/ModemManager1/Modem/0 | grep 'signal quality' | awk '{print $4}'";

            // Initialize a new process
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash"; // Use bash to run the command
            process.StartInfo.Arguments = $"-c \"{command}\""; // -c flag for bash to run the command
            process.StartInfo.RedirectStandardOutput = true; // Redirect standard output to read it
            process.StartInfo.UseShellExecute = false; // Do not use the shell to start the process
            process.StartInfo.CreateNoWindow = true; // Do not create a window for the process

            try
            {
                // Start the process
                process.Start();

                // Read the output
                string output = process.StandardOutput.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Print the output (signal strength)
                return int.Parse(output);   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
               
                return -1;
            }
        }

        public static string GetReturnString() 
        {
            ReturnString = ReturnString + GetSignalStrength().ToString();
            ReturnString = ReturnString + ErrorBool.ToString();
            return ReturnString;
        }
    }
}