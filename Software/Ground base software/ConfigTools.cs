using System;
using System.IO;
using System.Security.AccessControl;
using System.Text.Json;
using SharpDX.XInput;

namespace Ground_base_software
{
    public static class ConfigTools
    {
        private static List<Configuration> config = null;

        public static List<Configuration> GetConfigs()
        {
            if (config == null)
            {
                var Configs = new List<Configuration>();
                string Data = LoadConfigData();
                List<Configuration> configs = JsonSerializer.Deserialize<List<Configuration>>(Data);
                config = configs.ToList();
                return configs;
            }
            else 
            {
                return config;
            }

         
        }

        public static float GetAxisValue(string axisName, State state)
        {
            var gamepad = state.Gamepad;

            switch (axisName)
            {
                // Analog axes
                case "LeftThumbX": return gamepad.LeftThumbX / 32767f;
                case "LeftThumbY": return gamepad.LeftThumbY / 32767f;
                case "RightThumbX": return gamepad.RightThumbX / 32767f;
                case "RightThumbY": return gamepad.RightThumbY / 32767f;
                case "LeftTrigger": return gamepad.LeftTrigger / 255f;
                case "RightTrigger": return gamepad.RightTrigger / 255f;

                // Buttons (bitmask checks)
                case "A": return (gamepad.Buttons & GamepadButtonFlags.A) != 0 ? 1f : 0f;
                case "B": return (gamepad.Buttons & GamepadButtonFlags.B) != 0 ? 1f : 0f;
                case "X": return (gamepad.Buttons & GamepadButtonFlags.X) != 0 ? 1f : 0f;
                case "Y": return (gamepad.Buttons & GamepadButtonFlags.Y) != 0 ? 1f : 0f;
                case "DPadUp": return (gamepad.Buttons & GamepadButtonFlags.DPadUp) != 0 ? 1f : 0f;
                case "DPadDown": return (gamepad.Buttons & GamepadButtonFlags.DPadDown) != 0 ? 1f : 0f;
                case "DPadLeft": return (gamepad.Buttons & GamepadButtonFlags.DPadLeft) != 0 ? 1f : 0f;
                case "DPadRight": return (gamepad.Buttons & GamepadButtonFlags.DPadRight) != 0 ? 1f : 0f;
                case "Start": return (gamepad.Buttons & GamepadButtonFlags.Start) != 0 ? 1f : 0f;
                case "Back": return (gamepad.Buttons & GamepadButtonFlags.Back) != 0 ? 1f : 0f;
                case "LeftShoulder": return (gamepad.Buttons & GamepadButtonFlags.LeftShoulder) != 0 ? 1f : 0f;
                case "RightShoulder": return (gamepad.Buttons & GamepadButtonFlags.RightShoulder) != 0 ? 1f : 0f;
                case "LeftThumb": return (gamepad.Buttons & GamepadButtonFlags.LeftThumb) != 0 ? 1f : 0f;
                case "RightThumb": return (gamepad.Buttons & GamepadButtonFlags.RightThumb) != 0 ? 1f : 0f;

                default:
                    throw new ArgumentException($"Unknown input: {axisName}");
            }
        }


        private static string LoadConfigData()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            if (!File.Exists(configPath))
            {     
                 MessageBox.Show($"Config file not found {AppDomain.CurrentDomain.BaseDirectory}config.txt");
            }
            else
            {
                try
                {
                    using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\config.txt"))
                    {
                        return sr.ReadToEnd();
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("Config path invalid");
                    return null;
                }
                catch (DriveNotFoundException)
                {
                    MessageBox.Show("Drive not found");
                    return null;
                }
                catch (FileLoadException)
                {
                    MessageBox.Show("File couldnt be loaded. Likely still open in some editor...");
                    return null;
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Config file couldnt be found");
                    return null;
                }
                catch (InvalidDataException)
                {
                    MessageBox.Show("Invalid format of config file");
                    return null;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Exception occured: {e.Message} when loading config file");
                    return null;
                }
            }
            return null;
        }

    }

    public class Configuration
    {
        public int ServoAddress { get; set; }
        public string ControllerAxis { get; set; }
        public int MaxAngle { get; set; }
        public int MinAngle { get; set; }

        public Configuration(int servoAddress = 0, string controllerAxis = "X", int maxAngle = 180, int minAngle = 0)
        {
            ServoAddress = servoAddress;
            ControllerAxis = controllerAxis;
            MaxAngle = maxAngle;
            MinAngle = minAngle;
        }

        public void DisplayConfiguration()
        {
            Console.WriteLine($"Servo Address: {ServoAddress}");
            Console.WriteLine($"Controller Axis: {ControllerAxis}");
            Console.WriteLine($"Max Angle: {MaxAngle}");
            Console.WriteLine($"Min Angle: {MinAngle}");
        }

    }



}

    