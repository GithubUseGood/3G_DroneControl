using System.Globalization;

namespace ZOHD_airplane_software
{
    public static class UnpackMessageTools 
    {
        public static List<Instruction> UnpackMessages(string data)
        {
            var Instructions = new List<Instruction>();
            while (!string.IsNullOrEmpty(data)) // safer check
            {
                try
                {
                    Instruction instruction = new Instruction();

                    int start = data.IndexOf('Q');
                    int end = data.IndexOf('T');

                    string angleStr = data.Substring(start + 1, end - start - 1);
                    double angleDouble = double.Parse(angleStr, CultureInfo.InvariantCulture);
                    instruction.angle = (int)Math.Round(angleDouble);

                    data = data.Remove(start, end - start + 1);

                    start = data.IndexOf('O');
                    end = data.IndexOf('Q');

                    instruction.adress = int.Parse(data.Substring(start + 1, end - start - 1));
                    data = data.Remove(start, end - start + 1);

                    Instructions.Add(instruction);

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Couldnt convert data into instruction {e.Message}");
                    break;
                }
            }

            return Instructions;
        }





        public class Instruction 
        {
            public int angle { get; set; }
            public int adress { get; set; }
        }
    }
}