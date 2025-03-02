using Ground_base_software;
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



    }
}