using System.Diagnostics;
using System.Net;

namespace ZOHD_airplane_software 
{
    class OutputStatus() 
    {
        private static bool IsTesting;
        public static bool StopOutput { get; set; } = false;

        private static String SignalStrength = "0";
        private static String CPUtemp;
        private static String percentageOfRamUsed;
        private static String percentageOfCpuUsed;
       
       
        public static void Start() 
        {
            if (OperatingSystem.IsWindows())
            {
                Console.WriteLine("Windows OS detected, returning test data");
                IsTesting = true;
            }
            else 
            {
                IsTesting = false;
                Task.Run(() => UpdateData());
            }
          
        }

        public static string GetOutput() 
        {
            if (!IsTesting)
            {
                String data = SignalStrength + "." + CPUtemp + "q" + percentageOfCpuUsed + "w" + percentageOfRamUsed;
                string modifiedString = data.Replace("\n", "").Replace("\r", "");

                return modifiedString;
            }
            else 
            {
                return "-20";
            }
         
        }

      

        static async Task UpdateData() 
        {
            while (StopOutput != true) 
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

                   
                    SignalStrength = output;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");

                    
                }

                // get cpu temp 
                string commandCPU = "sensors | awk '/^cpu_thermal-virtual-0/ {found=1; next} found && /^temp1:/ {print $2}' | tr -d '+°C'";

                // Initialize a new process
                Process processCPU = new Process();
                processCPU.StartInfo.FileName = "/bin/bash"; // Use bash to run the command
                processCPU.StartInfo.Arguments = $"-c \"{commandCPU}\""; // -c flag for bash to run the command
                processCPU.StartInfo.RedirectStandardOutput = true; // Redirect standard output to read it
                processCPU.StartInfo.UseShellExecute = false; // Do not use the shell to start the process
                processCPU.StartInfo.CreateNoWindow = true; // Do not create a window for the process

                try
                {
                    // Start the process
                    processCPU.Start();

                    // Read the output
                    string outputCPU = processCPU.StandardOutput.ReadToEnd();
                    outputCPU = RoundToNearestInt(float.Parse(outputCPU)).ToString();
                    // Wait for the process to exit
                    processCPU.WaitForExit();

                    
                    CPUtemp = outputCPU;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");


                }

                string commandCPUusage = @"top -bn1 | grep 'Cpu(s)' | sed 's/.*, *\([0-9.]*\)%* id.*/\1/' | awk '{print 100 - $1}'";

                // Initialize a new process
                Process processCPUusage = new Process();
                processCPUusage.StartInfo.FileName = "/bin/bash"; // Use bash to run the command
                processCPUusage.StartInfo.Arguments = $"-c \"{commandCPUusage}\""; // -c flag for bash to run the command
                processCPUusage.StartInfo.RedirectStandardOutput = true; // Redirect standard output to read it
                processCPUusage.StartInfo.UseShellExecute = false; // Do not use the shell to start the process
                processCPUusage.StartInfo.CreateNoWindow = true; // Do not create a window for the process

                try
                {
                    // Start the process
                    processCPUusage.Start();

                    // Read the output
                    string CpuUsage = processCPUusage.StandardOutput.ReadToEnd();
                    CpuUsage = RoundToNearestInt(float.Parse(CpuUsage)).ToString();
                    // Wait for the process to exit
                    processCPUusage.WaitForExit();


                    percentageOfCpuUsed = CpuUsage;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");


                }

                string commandRAMusage = "free | grep Mem | awk '{print $3/$2 * 100.0}'";

                // Initialize a new process
                Process processRAMusage = new Process();
                processRAMusage.StartInfo.FileName = "/bin/bash"; // Use bash to run the command
                processRAMusage.StartInfo.Arguments = $"-c \"{commandRAMusage}\""; // -c flag for bash to run the command
                processRAMusage.StartInfo.RedirectStandardOutput = true; // Redirect standard output to read it
                processRAMusage.StartInfo.UseShellExecute = false; // Do not use the shell to start the process
                processRAMusage.StartInfo.CreateNoWindow = true; // Do not create a window for the process

                try
                {
                    // Start the process
                    processRAMusage.Start();

                    // Read the output
                    string RAMUsage = processRAMusage.StandardOutput.ReadToEnd();
                    RAMUsage = RoundToNearestInt(float.Parse(RAMUsage)).ToString();
                    // Wait for the process to exit
                    processRAMusage.WaitForExit();


                    percentageOfRamUsed = RAMUsage;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");


                }

                Thread.Sleep(1000);
            }
            
        }

        static int RoundToNearestInt(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }


    }
}