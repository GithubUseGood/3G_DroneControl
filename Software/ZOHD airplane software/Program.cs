using Iot.Device.Pwm;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using UDPtest;

namespace ZOHD_airplane_software
{
    internal class Program
    {
        private static int PortTXT = 9998;
        private static int PortVID = 9990;

        private static IPAddress HostIp = Dns.GetHostAddresses("z0hd.ddns.net")[0]; // no need to pay for an domain you can get some for free at noip.com
        private static IPAddress ProxyIp = Dns.GetHostAddresses("zohdserver.ddns.net")[0]; // no need to pay for an domain you can get some for free at noip.com


        private static UdpClient _client = new UdpClient(9999);
        private static UdpClient _clientVideo = new UdpClient(PortVID);
        private static Pca9685 ServoControllerPca;
        private static bool ReadData = true;
        private static bool SendData = true;

        private static string Key = "AVIJON1";
        private static int NegotiationPort = 9900;
        private static UdpClient NegotiationClient = new UdpClient();
        private static String RecievedMessage;



        static async Task Main(string[] args)
        {
            Console.WriteLine($"Welcome {DateTime.Now}\n Would you like to use a Proxy? Y/N");
            string response = Console.ReadLine();
            if (response == "Y" || response == "y") 
            {
                NegotiationClient.Connect(ProxyIp, NegotiationPort);
                await NegotiationClient.SendAsync(Encoding.UTF8.GetBytes(Key));
                Console.WriteLine("Sent key to proxy");
                UdpReceiveResult result = await NegotiationClient.ReceiveAsync();
                List<int> Ports = UDP_Communication.ExtractPorts(Encoding.UTF8.GetString(result.Buffer));
                PortTXT = Ports[0];
                PortVID = Ports[1];
                HostIp = ProxyIp;

               
            }


           
            ServoControllerPca = Controls.ConnectServoController();
            Console.WriteLine("HOST:" + HostIp.ToString() + " PORTS: VID: " + PortVID);
            OutputStatus.Start();
            StartUDPcomms();

            while (true) ;
        }

        static async Task StartUDPcomms() 
        {
            _client.Connect(HostIp, PortTXT);
           
            Console.WriteLine("sending UDP to: " + HostIp + ":" + PortTXT);
            Console.WriteLine("reading UDP on: any ip and on port " + PortTXT);
            Task.Run(() => SendUDP(_client));
            Task.Run(() => RecieveUDP(_client));
            Task.Run(() => UDP_Communication.SendVideo(HostIp + ":" + PortVID.ToString()));


        }
        static async Task SendUDP(UdpClient _client)
        {
            while (SendData) 
            {
           
               await UDP_Communication.SendUDP(_client, OutputStatus.GetOutput());
                Thread.Sleep(30);
            }
        }
        static async Task RecieveUDP(UdpClient _client)
        {
            while (ReadData)
            {
               RecievedMessage = await UDP_Communication.ReadUDP(_client);
               Controls.WriteServoData(RecievedMessage, ServoControllerPca);
            
            }
        }

      

        


    }
}
