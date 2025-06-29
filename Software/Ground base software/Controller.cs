namespace PeerToPeerTest 
{
    using System.IO;
    using System.IO.Ports;
    using Ground_base_software;
    using SharpDX.XInput;
    using ZOHD_airplane_software;

    public static class controllerClass
    {
        public static Controller ConnectControler(bool FastConnect=false)
        {
            Controller controller = new Controller(UserIndex.One);
           
            try
            {
                controller = new Controller(UserIndex.One);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e + " Press any key to attempt again");
                ConnectControler();
            }
            if (controller.IsConnected != true)
            {
                if (FastConnect == true)
                {
                    MessageBox.Show("Controller not found Retrying now");
                    ConnectControler();
                }
                else
                {
                    MessageBox.Show("Controller not found Retrying in 5 seconds");
                    Thread.Sleep(5000);
                    ConnectControler();
                }
            }
            return controller;
        }

        public static string GetOutput(Controller controller) // used to get controller state as a string
        {
            if (controller.IsConnected == false) 
            {
               controller = ConnectControler(true);
            }

            State state;
            state = controller.GetState();
            string message = "";
            var configs = ConfigTools.GetConfigs();
            foreach (var config in configs) 
            {
                message = message + $"Q{MapValue(ConfigTools.GetAxisValue(config.ControllerAxis, state), 0, 1, config.MinAngle, config.MaxAngle)}TO{config.ServoAddress}"; 
            }
            
            /*
            message = MapValue(state.Gamepad.RightThumbY, -32767, 32767, 1, 360) + "q"
             + MapValue(state.Gamepad.RightThumbX, -32767, 32767, 1, 180) + "w"
              + MapValue(state.Gamepad.LeftThumbY, -32767, 32767, 1, 180) + "e"
               + MapValue(state.Gamepad.LeftThumbX, -32767, 32767, 1, 180) + "r";
            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y))
            {
                message = message + "T";
            }
            else
            {
                message = message + "F";
            }
            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B))
            {
                message = message + "T";
            }
            else
            {
                message = message + "F";
            }
            */

            return message;

        }

        static float MapValue(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
        {
            return ((oldValue - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
        }

    }
}