using System;

namespace InputProcessor
{
    public class DataHandler
    {
        private int num1;
        private int num2;
        
        public DataHandler(int num1, int num2)
        {
            this.num1 = num1;
            this.num2 = num2;
        }

        public int AddNumbers()
        {
            int result = num1 + num2;
            Console.WriteLine($"Adding numbers: {num1} + {num2} = {result}");
            return result;
        }
    }
}