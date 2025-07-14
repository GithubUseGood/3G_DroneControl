using Ground_base_software;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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

        public static void FlushUDP(UdpClient client)
        {

            var remoteEP = new IPEndPoint(IPAddress.Any, 0);

            // Set ReceiveTimeout to avoid blocking indefinitely
            client.Client.ReceiveTimeout = 10; // milliseconds

            while (client.Available > 0)
            {
                try
                {
                    byte[] data = client.Receive(ref remoteEP);
                    // Discard data
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.TimedOut ||
                        ex.SocketErrorCode == SocketError.WouldBlock)
                    {
                        break; // No more data to read
                    }
                    else
                    {
                        throw; // Unexpected socket error
                    }
                }
            }

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

        public static SshClient SSHOpenConnection()
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

        private static readonly ManualResetEventSlim uiResponseEvent = new(false);
        private static readonly object _lock = new();
        private static int _lastLatencyMs = -1;
        private static DateTime _lastMeasureTime = DateTime.MinValue;

        public static int MeasureUiResponseTime(Control uiControl, int timeoutMs = 500)
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                if ((now - _lastMeasureTime).TotalSeconds < 1)
                {
                    // Return cached result if measured less than 1 second ago
                    return _lastLatencyMs;
                }

                _lastMeasureTime = now;
            }

            uiResponseEvent.Reset();
            var stopwatch = Stopwatch.StartNew();

            if (!uiControl.InvokeRequired)
            {
                // On UI thread: respond immediately
                lock (_lock)
                {
                    _lastLatencyMs = 0;
                    _lastMeasureTime = DateTime.UtcNow;
                }
                return 0;
            }

            uiControl.BeginInvoke(new Action(() =>
            {
                stopwatch.Stop();
                uiResponseEvent.Set();
            }));

            bool signaled = uiResponseEvent.Wait(timeoutMs);

            lock (_lock)
            {
                _lastLatencyMs = signaled ? (int)stopwatch.ElapsedMilliseconds : -1;
                _lastMeasureTime = DateTime.UtcNow;
            }

            return _lastLatencyMs;
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

        public static async Task ResetIfMessagesDropToZero(UdpClient client)
        {
            Thread.Sleep(2500); // give time for everything to initalise to avoid reseting if its not needed
            while (true)
            {
                if (UDP_Communication.MessagesPerSecond == 0)
                {
                    Form1.ReadData = false;
                    await Task.Delay(4000);
                    Form1.ReadData = true;

                    // Fire-and-forget wrapped in Task.Run
                    _ = Task.Run(() => Form1.RecieveUDP(client));
                }

                await Task.Delay(5000);
            }
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

        public class SSHBox
        {
            private static ShellStream stream;
            private static StringBuilder outputLog = new StringBuilder();
            private static bool isReading = false;
            private static RichTextBox _sshOutputLabel;
            private static readonly int MaxLogLength = 100_000;

            public static void StartSSH(SshClient client)
            {
                client.Connect();

                if (!client.IsConnected)
                {
                    MessageBox.Show($"Failed to connect to UAV");
                    return;
                }

                stream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
                stream.WriteLine("cd ~/TESTcode && ./ZOHD");

                if (!isReading)
                {
                    isReading = true;
                    Task.Run(() => ReadOutputLoop());
                }

                Thread.Sleep(2500); // give time for UAV to init
            }

           

            public static void SendCommand(string command)
            {
                if (stream != null && stream.CanWrite)
                    stream.WriteLine(command);
            }

            public static void SendCtrlC()
            {
                if (stream != null && stream.CanWrite)
                {
                    stream.Write("\x03"); // Ctrl+C character
                    stream.Flush();
                }
            }

            public static string GetFullOutput()
            {
                return outputLog.ToString();
            }

            public static void SetOutputLabel(RichTextBox richTextBox)
            {
                _sshOutputLabel = richTextBox;
            }

            private static void ReadOutputLoop()
            {
                while (true)
                {
                    if (stream != null && stream.DataAvailable)
                    {
                        string data = stream.Read();
                        if (!string.IsNullOrEmpty(data))
                        {
                            outputLog.Append(data);

                            if (_sshOutputLabel != null && _sshOutputLabel.IsHandleCreated)
                            {
                                _sshOutputLabel.Invoke((MethodInvoker)(() =>
                                {
                                    _sshOutputLabel.Text = Regex.Replace(outputLog.ToString(), @"\x1B\[[0-9;]*[mK]", "");

                                    // Auto-scroll to the bottom:
                                    _sshOutputLabel.SelectionStart = _sshOutputLabel.Text.Length;
                                    _sshOutputLabel.ScrollToCaret();

                                    if (outputLog.Length > MaxLogLength)
                                    {
                                        outputLog.Remove(0, outputLog.Length / 4);
                                    }
                                }));
                            }
                        }
                    }
                    Thread.Sleep(30);
                }
            }



        }
    }
}