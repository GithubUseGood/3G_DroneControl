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
        private static IPEndPoint TailscaleEndPoint;
        private static IPAddress TailScaleIP;
        private static UdpClient _client = new UdpClient(PortTXT);
        private static UdpClient _clientVideo = new UdpClient(PortVID);
        private static Pca9685 ServoControllerPca;
        private static bool ReadData = true;
        private static bool SendData = true;
        private static int NegotiationPort = 9900;
        private static UdpClient NegotiationClient = new UdpClient();
        private static String RecievedMessage;



        static async Task Main(string[] args)
        {
          
            Console.WriteLine($"Welcome {DateTime.Now}");
            TailscaleEndPoint = await UDP_Communication.CaptureIpFromMessage(_client);
            TailScaleIP = TailscaleEndPoint.Address;
            ServoControllerPca = Controls.ConnectServoController();
            if (ServoControllerPca == null) { ServoControllerPca = Controls.ConnectServoController(); } // retry once
            Console.WriteLine("HOST:" + TailScaleIP.ToString() + " PORTS: VID: " + PortVID);
            OutputStatus.Start();
            StartUDPcomms();

            while (true) ;
        }

        static async Task StartUDPcomms() 
        {
            _client.Connect(TailScaleIP, PortTXT);
           
            Console.WriteLine("sending UDP to: " + TailScaleIP + ":" + PortTXT);
            Console.WriteLine("reading UDP on: any ip and on port " + PortTXT);
            Task.Run(() => SendUDP(_client));
            Task.Run(() => RecieveUDP(_client));
            Task.Run(() => UDP_Communication.SendVideo(TailScaleIP + ":" + PortVID.ToString()));


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
