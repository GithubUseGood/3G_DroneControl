using AxWMPLib;
using LibVLCSharp.Shared;

using PeerToPeerTest;
using SharpDX.XInput;
using System.Net;
using System.Net.Sockets;
using UDPtest;
using WMPLib;
using ZOHD_airplane_software;

using System.Runtime.CompilerServices;
using Vlc.DotNet.Forms;
using System.Diagnostics;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Text;
using Renci.SshNet;
namespace Ground_base_software
{
    public partial class Form1 : Form
    {


        private static SshClient SSHclient;
        private static string TailscaleIP = "uav.tail5bbbe4.ts.net";
        private static IPEndPoint tailscaleEndPoint;
        private static int PortTXT = 9998;
        private static int PortVID = 9990;
        private static int NegotiationPort = 9901;

        private static UdpClient NegotiationClient = new UdpClient();

        private static UdpClient _client = new UdpClient(PortTXT);

        private static UdpClient _NATpuncherTXT = new UdpClient();
        private static UdpClient _NATpuncherVID = new UdpClient();

        private static bool ReadData = true;
        private static bool SendData = true;

        private static String RecievedMessage;

        private static Label label;
        private static Label Label2;
        private static Label Label5;
        private static Label Label6;
        private static Label Label7;
        private static Label IPlabel;

        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;




        public Form1()
        {
            InitializeComponent();


        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            IPlabel = this.label7;
            label = this.label1;
            Label2 = this.label4;
            Label5 = this.label5;
            Label6 = this.label8;
            Label7 = this.label9;
            Label5.Text = "Loading...";
            label.Text = "Loading...";
            Label2.Text = "Loading...";
            Label6.Text = "Loading...";
            Label7.Text = "Loading...";
            IPlabel.Text = "Loading...";


            UDP_Communication.TailScale.Up();

            using (var connecting = new Form2()) // temp form to tell user its connecting
            {
                connecting.Show();
                connecting.Refresh();

                SSHclient = UDP_Communication.SSHOpenConnection();


                UDP_Communication.StartScriptOnUAV(SSHclient);


                connecting.Close();
            }



            Task.Run(() => StartPlayback(PortVID.ToString()));
            StartUDPcomms();


        }

        static async Task StartUDPcomms()
        {
            Controller controler = controllerClass.ConnectControler();

            tailscaleEndPoint = await UDP_Communication.CaptureIpFromMessage(_client);
            _client.Connect(tailscaleEndPoint);

            Task.Run(() => SendUDP(_client, controler));
            Task.Run(() => RecieveUDP(_client));
            UDP_Communication.TrackMessagesPerSecond();

            Thread.Sleep(1000);



        }



        static async Task SendUDP(UdpClient _client, Controller controler)
        {
            while (SendData)
            {
                await UDP_Communication.SendUDP(_client, controllerClass.GetOutput(controler));
                // MessageBox.Show(controllerClass.GetOutput(controler));
                Thread.Sleep(10);
            }
        }
        static async Task RecieveUDP(UdpClient _client)
        {
            RecievedMessage = await UDP_Communication.ReadUDP(_client);
            Task.Run(() => UpdateUI());
            while (ReadData)
            {
                RecievedMessage = await UDP_Communication.ReadUDP(_client);
            }
        }
        private static async Task StartPlayback(string streamUri)
        {
            string command = "ffplay";

            string arguments = $"-fflags nobuffer -flags low_delay -framedrop -strict experimental -an -probesize 3M -analyzeduration 1M -i udp://0.0.0.0:{PortVID}";


            // Create a new process
            Process process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;

            // Start the process
            process.Start();

            // Optionally read the output
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Wait for the process to exit
            process.WaitForExit();

            // Display output (if any)
            MessageBox.Show("Output: ");
            MessageBox.Show(output);

            MessageBox.Show("Error: ");
            MessageBox.Show(error);
        }



        private void label1_Click(object sender, EventArgs e)
        {


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }


        private void Form1_Shown(object sender, EventArgs e)
        {

        }



        private static void UpdateUI()
        {
            while (true)
            {
                String lastString = RecievedMessage;
                int separatorIndex = lastString.IndexOf('.');
                if (separatorIndex != -1)
                {
                    try
                    {
                        if (IsLabelSafeToEdit(label))
                        {
                            int[] borders = { 0, 39, 40, 80, 100 };
                            label.Invoke(new Action(() => label.Text = lastString.Substring(0, lastString.IndexOf("."))));
                            label.Invoke(new Action(() => label.ForeColor = UDP_Communication.AsignColor(UDP_Communication.ParseAtAllCosts(lastString.Substring(0, lastString.IndexOf("."))), borders)));
                        }
                        else
                        {

                        }


                    }
                    catch (Exception ex)
                    {
                        label.ForeColor = Color.Purple;

                    }
                    try
                    {
                        if (IsLabelSafeToEdit(Label2))
                        {
                            int[] borders = { 0, 44, 45, 90, 100 };

                            Label2.Invoke(new Action(() => Label2.Text = UDP_Communication.MessagesPerSecond.ToString()));
                            Label2.Invoke(new Action(() => Label2.ForeColor = UDP_Communication.AsignColor(UDP_Communication.MessagesPerSecond, borders)));
                        }
                        else
                        {

                        }

                    }
                    catch (Exception ex)
                    {
                        Label2.ForeColor = Color.Purple;
                    }

                    try
                    {

                        if (IsLabelSafeToEdit(Label5))
                        {

                            int[] borders = { 0, 44, 45, 90, 100 };
                            Label5.Invoke(new Action(() => Label5.Text = lastString.Substring(lastString.IndexOf('.') + 1, (lastString.IndexOf('q') - lastString.IndexOf('.') - 1))));
                            Label5.Invoke(new Action(() => Label5.ForeColor = UDP_Communication.AsignColor(UDP_Communication.ParseAtAllCosts(lastString.Substring(lastString.IndexOf('.') + 1, (lastString.IndexOf('q') - lastString.IndexOf('.') - 1))), borders)));
                        }
                        else
                        {

                        }



                    }
                    catch (Exception ex)
                    {
                        Label5.ForeColor = Color.Purple;
                        Label5.Invoke(new Action(() => Label5.Text = ex.Message));
                    }

                    try
                    {

                        if (IsLabelSafeToEdit(Label6))
                        {

                            int[] borders = { 90, 100, 44, 89, 0 };
                            Label6.Invoke(new Action(() => Label6.Text = lastString.Substring(lastString.IndexOf('q') + 1, (lastString.IndexOf('w') - lastString.IndexOf('q') - 1))));
                            Label6.Invoke(new Action(() => Label6.ForeColor = UDP_Communication.AsignColor(UDP_Communication.ParseAtAllCosts(lastString.Substring(lastString.IndexOf('q') + 1, (lastString.IndexOf('w') - lastString.IndexOf('q') - 1))), borders)));
                        }
                        else
                        {

                        }



                    }
                    catch (Exception ex)
                    {
                        Label6.ForeColor = Color.Purple;
                        Label6.Invoke(new Action(() => Label6.Text = ex.Message));
                    }

                    try
                    {

                        if (IsLabelSafeToEdit(Label7))
                        {

                            int[] borders = { 90, 100, 44, 89, 0 };
                            Label7.Invoke(new Action(() => Label7.Text = lastString.Substring(lastString.IndexOf('w') + 1)));
                            Label7.Invoke(new Action(() => Label7.ForeColor = UDP_Communication.AsignColor(UDP_Communication.ParseAtAllCosts(lastString.Substring(lastString.IndexOf('w') + 1)), borders)));
                        }
                        else
                        {

                        }



                    }
                    catch (Exception ex)
                    {
                        Label7.ForeColor = Color.Purple;
                        Label7.Invoke(new Action(() => Label7.Text = ex.Message));
                    }

                    try
                    {
                        if (IsLabelSafeToEdit(IPlabel))
                        {
                            //  IPlabel.Invoke(new Action(() => IPlabel.Text=IPlabel.ToString()));
                            IPlabel.Invoke(new Action(() => IPlabel.Text = TailscaleIP.ToString()));
                            IPlabel.Invoke(new Action(() => IPlabel.ForeColor = Color.Green));
                        }
                    }
                    catch (Exception ex)
                    {
                        IPlabel.ForeColor = Color.Red;
                        IPlabel.Invoke(new Action(() => IPlabel.Text = ex.Message));
                    }
                }
                else
                {
                    Environment.Exit(2);
                }
            }

        }

        private static bool IsLabelSafeToEdit(Label label)
        {
            if (label == null)
            {
                // The label is null
                MessageBox.Show("BOX is null m8");
                return false;
            }

            if (label.IsDisposed)
            {
                // The label has been disposed
                MessageBox.Show("Why did someone throw this away");
                return false;
            }

            if (!label.IsHandleCreated)
            {
                // The label's handle has not been created
                MessageBox.Show("Handle missin");
                return false;
            }

            if (!label.Visible)
            {
                // The label is not visible
                MessageBox.Show("Label aint visibl");
                return false;
            }

            return true;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartPlayback(PortVID.ToString());
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UDP_Communication.TailScale.Down();
        }

        private void OpenSSH_Click(object sender, EventArgs e)
        {
            UDP_Communication.OpenSSHinCMD();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UDP_Communication.OpenSSHinCMD();
        }
    }
}
