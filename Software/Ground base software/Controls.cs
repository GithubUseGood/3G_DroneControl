using Iot.Device.ServoMotor;
using System;
using System.Device.Gpio;
using System.Runtime.CompilerServices;

namespace UDPtest 
{
    

    public static class Controls
    {
        public static GpioController Gpio { get; set; }
        private static int AlerionsPin = 0;
        private static int ElevatorsPin = 0;
        private static int ThrottlePin = 0;
        private static GpioPin AlerionServo;
        private static GpioPin ElevatorsServo;
        private static GpioPin ThrottleServo;
        public static void InitGpio() 
        {
             Gpio = new GpioController();
        }
     


        public static void InitControls() 
        {
       
            try 
            {
                AlerionServo = Gpio.OpenPin(AlerionsPin, PinMode.Output);
            }
            catch 
            {
                Information.ErrorSwitch();
            }
            try
            {
                 ElevatorsServo = Gpio.OpenPin(ElevatorsPin, PinMode.Output);
            }
            catch
            {
                Information.ErrorSwitch();
            }
            try
            {
                ThrottleServo = Gpio.OpenPin(ThrottlePin, PinMode.Output);
            }
            catch
            {
                Information.ErrorSwitch();
            }

       
    

        }

        public static void WriteControls(String input) 
        {
            int alerionsPos;
            int elevatorPos;
            int throttlePos;
            try
            {
                throttlePos = int.Parse(input.Substring(0, input.IndexOf("x")));
                ThrottleServo.Write(throttlePos);
             
            }
            catch { Information.ErrorSwitch(); }
            try 
            {

                alerionsPos = int.Parse(input.Substring(input.IndexOf("w"), input.IndexOf("e")));
                AlerionServo.Write(alerionsPos);
             
            } catch { Information.ErrorSwitch(); }
            try 
            {
                elevatorPos = int.Parse(input.Substring(input.IndexOf("e"), input.IndexOf("r")));
                ElevatorsServo.Write(elevatorPos);
            }catch { Information.ErrorSwitch(); }
         
        }
    }
}