using Ground_base_software;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ZOHD_airplane_software
{ 
    
    class UDP_Communication() 
    {
        private static int CurrentMessages = 0;
       
        public static int MessagesPerSecond { get; set; } = 0;

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
            CurrentMessages++;
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

        public static async Task TrackMessagesPerSecond() 
        {
            CurrentMessages = 0;
            while (true) 
            {
                MessagesPerSecond = CurrentMessages;
                CurrentMessages = 0;
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public static PingReply CheckIfReachable(string ipv4) 
        {
            using (Ping ping = new Ping()) 
            {
                PingReply reply = ping.Send(ipv4, 1000); // 1000ms timeout
                return reply;
            }
        }


      

        public static Color AsignColor(int value, int[] Borders) 
        {
            try
            {
                switch (value)
                {
                    case int n when (Borders[1] >= value && Borders[0] <= value):
                        return Color.Red;
                        break;

                    case int n when (Borders[2] >= value && Borders[1] <= value):
                        return Color.Orange;
                        break;

                    case int n when (Borders[3] >= value && Borders[2] <= value):
                        return Color.Green;
                        break;

                    case int n when (Borders[4] >= value && Borders[3] <= value):
                        return Color.Blue;
                        break;

                    default: return Color.Purple;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in color assignment: " + ex.Message);
                return Color.Purple;
            }
        
        }

        public static int ParseAtAllCosts(string ToParse) 
        {
            try
            {
                String ParsedData = "";
                foreach (char c in ToParse)
                {
                    if (char.IsDigit(c))
                    {
                        ParsedData += c;
                    }
                }
                if (ToParse == String.Empty) return -1;
                return int.Parse(ParsedData);
            }
            catch 
            {
                return -1;
            }
         
        }

        public static async Task KeepNATopen(UdpClient _client1, UdpClient _client2) // heartbeat
        {
            Byte[] Message = Encoding.ASCII.GetBytes("Keep me alive cuh");
            while (true) 
            {
               await _client1.SendAsync(Message);
               await _client2.SendAsync(Message);

                Thread.Sleep(5000);
            }
           
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

        public static void OpenSSHinCMD() 
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ssh",
                        Arguments = $"User1@{GetIPofUAV()}",
                        RedirectStandardOutput = false,
                        UseShellExecute = false,
                        CreateNoWindow = false
                    }
                };

                // Start the process and capture the output
                process.Start();
            }
            catch 
            {
                MessageBox.Show("SSH failed. Likely not installed on your machine. Or make sure it is in PATH");
            }
        }

        public static SshClient SSHOpenConnection ()
        {
            string hostname = GetIPofUAV();
            string username = "User1";
            string password = "1";

            var client = new SshClient(hostname, username, password);
            KillOldRunTime(client);
            return client;

        }

        private static void KillOldRunTime(SshClient client) 
        {
            client.Connect();
            client.RunCommand("killall ZOHD");
            client.Disconnect();
        }


        public static void StartScriptOnUAV(SshClient client) 
        {
            try
            {
                client.Connect();
            }

            catch (Exception e)
            {
                MessageBox.Show($"Failed to connect to UAV. Exceotion {e.Message} either quit, or start the script manually via SSH");
            }


            using (var cmd = client.CreateCommand("nohup ./TESTcode/ZOHD > zohd.out 2>&1 &"))
            {
                cmd.Execute();
            }
            client.Disconnect();
        }

        private static string GetIPofUAV()
        {
            string hostname = null;
            while (hostname == null) 
            {
                hostname = GetFirstOnlineMachineIp();
                if (hostname == null) 
                {
                    var res = MessageBox.Show("No online machines found.", "Error", MessageBoxButtons.RetryCancel);
                    if (res == DialogResult.Cancel) Environment.Exit(0);

                }
            }
            return hostname;
        }

        public static string GetFirstOnlineMachineIp()
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

                // Parse the output and find the first online machine's IP
                var lines = output.Split('\n');

                foreach (var line in lines)
                {
                    if (!line.Contains("offline") && !line.Contains(GetLocalTailscaleIp()))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 0)
                        {
                            string ip = parts[0];
                            Console.WriteLine($"Detected IP: {ip}");
                            return ip;
                        }
                    }
                }

                Console.WriteLine("No online machine detected");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
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

        public class TailScale
        {
            public static void Up()
            {
                RunCommand("tailscale", "up");
            }

            public static void Down()
            {
                RunCommand("tailscale", "down");
            }

            private static void RunCommand(string command, string arguments)
            {
                try
                {
                    using var process = new Process();
                    process.StartInfo.FileName = command;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(output))
                        Console.WriteLine("Output: " + output);

                    if (!string.IsNullOrWhiteSpace(error))
                        Console.WriteLine("Error: " + error);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception running command: " + ex.Message);
                }
            }
        }

    }
}