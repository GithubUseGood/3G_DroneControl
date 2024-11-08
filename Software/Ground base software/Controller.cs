namespace PeerToPeerTest 
{
    using System.IO.Ports;
    using SharpDX.XInput;
    public static class controllerClass
    {
        public static Controller ConnectControler()
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
            if (controller.IsConnected == true)
            {
            

            }
            else
            {
                MessageBox.Show("Controller not found Retrying in 5 seconds");
              
                Thread.Sleep(5000);
                ConnectControler();
            }
            return controller;
        }

        public static string GetOutput(Controller controller) // used to get controller state as a string
        {
            State state;
            state = controller.GetState();
            string message = "";
            message = MapValue(state.Gamepad.LeftThumbY, -32767, 32767, 1, 180) + "q"
             + MapValue(state.Gamepad.LeftThumbX, -32767, 32767, 1, 180) + "w"
              + MapValue(state.Gamepad.RightThumbX, -32767, 32767, 1, 180) + "e"
               + MapValue(state.Gamepad.RightThumbY, -32767, 32767, 1, 180) + "r";
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

            return message;

        }

        static int MapValue(int oldValue, int oldMin, int oldMax, int newMin, int newMax)
        {
            return (int)(((double)(oldValue - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin);
        }
    }
}