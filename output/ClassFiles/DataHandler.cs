using System;

namespace InputProcessor
{
    public interface IDataHandler
    {
        int ProcessData();
    }

    public class DataHandler : IDataHandler
    {
        private readonly int num1;
        private readonly int num2;
        
        public DataHandler(int number1 = 10, int number2 = 20)
        {
            num1 = number1;
            num2 = number2;
        }

        public int ProcessData()
        {
            try
            {
                Console.WriteLine("Processing Data...");
                int result = num1 + num2;
                Console.WriteLine("Calculated Result: " + result);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing data: " + ex.Message);
                throw;  // Rethrow to handle or log by caller
            }
        }
    }
}