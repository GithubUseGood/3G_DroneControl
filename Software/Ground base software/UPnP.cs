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
            
            NatUtility.DeviceFound += DeviceFound;
           
           

            try
            {
                NatUtility.StartDiscovery();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.ToString() + " Press any key to attempt again");
               
            
                Setup();
            }
        
                      
        }

        private static void DeviceFound(object sender, DeviceEventArgs args) 
        {
           device = args.Device;
         
          IsDeviceFound = true;
        
        }

        public static Mapping PortForward(int PortExternal, int PortInternal)
        {
          
            Mapping mapping;
            try
            {
                
                
                if (!Upnp.IsPortForwarded(PortInternal, PortExternal))
                {
                    mapping = new Mapping(Protocol.Udp, PortInternal, PortExternal);

                    device.CreatePortMap(mapping);
               
                    return mapping;
                }
                else 
                {
                   
                  
                    return null;
                  
                }
                

            } catch (Exception ex) 
            {
                MessageBox.Show("Error when creating port forward " + ex + " Press any key to attempt again");
                System.Windows.Forms.Clipboard.SetText(ex.ToString());

                PortForward(PortExternal, PortInternal);
                return null;
            }

        }

        public static bool IsPortForwarded(int portInternal, int portExternal) 
        {
            foreach (Mapping map in device.GetAllMappings()) 
            {
                if (map.PrivatePort == portInternal) 
                {
                    return true;
                }
            }
            foreach (Mapping map in device.GetAllMappings())
            {
                if (map.PublicPort == portExternal)
                {
                    return true;
                }
            }
            return false;
        }

        public static void PortForwardDelete(Mapping PortToDel) 
        {
            device.DeletePortMap(PortToDel);
        }

        public static async Task WaitForDeviceConnection() 
        {
            while (!Upnp.IsDeviceFound) await Task.Delay(100);
        }

        public static void DisplayAll() 
        {
            foreach (Mapping map in device.GetAllMappings())
            {
                MessageBox.Show("FOUND: " + map.PrivatePort.ToString());
            }
        }

        public static void DeleteAll() 
        {
            foreach (Mapping map in device.GetAllMappings()) 
            {
                device.DeletePortMap(map);
            }
        }
       
    }
}