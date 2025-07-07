using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ZOHD_airplane_software
{ 
    
    class UDP_Communication() 
    {

        public static string GetFirstOnlineMachineHostname()
        {
            try
            {
                // Run the tailscale status command
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "tailscale",
                        Arguments = "status",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                // Start the process and capture the output
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Print raw output for debugging
                Console.WriteLine("Tailscale Status Output:");
                Console.WriteLine(output);

                // Parse the output and find the first online machine's hostname
                var lines = output.Split('\n');

                foreach (var line in lines)
                {
                    if (!line.Contains("offline") && !line.Contains(GetLocalTailscaleIp()))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1)
                        {
                            string hostname = parts[1];
                            Console.WriteLine($"Detected: {hostname}");
                            return hostname;
                        }
                        else 
                        {
                            Console.WriteLine("fuck chatgpt");
                        }
                    }
                }

                Console.WriteLine("No online machine detected");
                // If no online machine is found
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns IPEndPoint from UDP message
        /// </summary>
        public static async Task<IPEndPoint> CaptureIpFromMessage(UdpClient _client) 
        {
            UdpReceiveResult result;
            result = await _client.ReceiveAsync();
            return result.RemoteEndPoint;
        }

        public static string GetLocalTailscaleIp()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tailscale",
                    Arguments = "ip --4",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            // The output may contain multiple IPs, so return the first one
            return output.Split('\n')[0].Trim();
        }

        public static List<int> ExtractPorts(string message)
        {
            List<int> ports = new List<int>();
            MatchCollection matches = Regex.Matches(message, @"\d+");

            foreach (Match match in matches)
            {
                ports.Add(int.Parse(match.Value));
            }

            return ports;
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
                string command = "ffmpeg";

                string arguments = $"-f v4l2 -framerate 30 -video_size 1280x720 -i /dev/video0 " +
                                   $"-vcodec libx264 -preset ultrafast -tune zerolatency " +
                                   $"-b:v 600k -f mpegts udp://{ipAndPort}";

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