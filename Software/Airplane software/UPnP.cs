using Mono.Nat;
using System.Net.NetworkInformation;

namespace UDPtest
{
    using Microsoft.Win32.SafeHandles;
    using Mono.Nat;
    public static class Upnp
    {
        private static Mono.Nat.INatDevice device;
        public static bool IsDeviceFound=false;

       

        public static async void Setup() 
        {
            Console.WriteLine("Starting Setup");
            NatUtility.DeviceFound += DeviceFound;
            Console.WriteLine("Starting Discovery");
           

            try
            {
                NatUtility.StartDiscovery();
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
                Console.WriteLine("Press any key to attempt again");
                Console.ReadKey();
                Setup();
            }
        
                      
        }

        private static void DeviceFound(object sender, DeviceEventArgs args) 
        {
           device = args.Device;
            Console.WriteLine("Router upnp found!");
          IsDeviceFound = true;
        
        }

        public static Mapping PortForward(int PortExternal, int PortInternal)
        {
            try
            {
                Mapping mapping = new Mapping(Protocol.Udp, PortInternal, PortExternal);

                device.CreatePortMap(mapping);
                return mapping;
            } catch (Exception ex) 
            {
                Console.WriteLine("Error when creating port forward " + ex);
                Console.WriteLine("Press any key to attempt again");
                Console.ReadKey();
                PortForward(PortExternal, PortInternal);
                return null;
            }

        }

        public static void PortForwardDelete(Mapping PortToDel) 
        {
            device.DeletePortMap(PortToDel);
        }


       
    }
}