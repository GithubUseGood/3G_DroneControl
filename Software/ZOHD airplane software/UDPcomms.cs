using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ZOHD_airplane_software
{ 
    
    class UDP_Communication() 
    {

        /// <summary>
        /// Returns IPEndPoint from UDP message
        /// </summary>
        public static async Task<IPEndPoint> CaptureIpFromMessage(UdpClient _client) 
        {
            UdpReceiveResult result;
            result = await _client.ReceiveAsync();
            return result.RemoteEndPoint;
        }

        /// <summary>
        /// Reads message from UDP.
        /// </summary>
        public static async Task<string> ReadUDP(UdpClient _client) 
        {
           
            UdpReceiveResult result;
            result = await _client.ReceiveAsync();
           
            return Encoding.ASCII.GetString(result.Buffer);
        }

        /// <summary>
        /// Sends message from UDP.
        /// Make sure UdpClient is connected
        /// </summary>
        public static async Task SendUDP(UdpClient _client, string message) 
        {
           await _client.SendAsync(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Send video over UDP using libcamera
        /// </summary>
        public static async Task SendVideo(string ipAndPort)
        {
            if (!OperatingSystem.IsWindows())
            {
                string command = "libcamera-vid";
                string arguments = @"libcamera-vid -t 0 --width 1280 --height 720 --framerate 30 --bitrate 870000 --inline --nopreview --output udp://" + ipAndPort;
                Console.WriteLine("Sending UDP video to " + ipAndPort);
                Console.WriteLine(command + " " + arguments);

                // Create a new process
                Process process = new Process();
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                // Start the process
                process.Start();

                // Optionally read the output
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Display output (if any)
                Console.WriteLine("Output: ");
                Console.WriteLine(output);

                Console.WriteLine("Error: ");
                Console.WriteLine(error);
            }
            else 
            {
                Console.WriteLine("Entering video stream simulator");
                string command = "ffmpeg";
                string arguments = "-stream_loop -1 -re -i  -c copy -f mpegts udp://127.0.0.1:9995"; // add path for your test video. If run on windows it will asume you are testing UDP video locally and send whatever path you add here.
              
                Console.WriteLine("Sending UDP video to " + ipAndPort);
                Console.WriteLine(command + " " + arguments);

                // Create a new process
                Process process = new Process();
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                // Start the process
                process.Start();

                // Optionally read the output
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Display output (if any)
                Console.WriteLine("Output: ");
                Console.WriteLine(output);

                Console.WriteLine("Error: ");
                Console.WriteLine(error);

            }


          

        }

    }
}