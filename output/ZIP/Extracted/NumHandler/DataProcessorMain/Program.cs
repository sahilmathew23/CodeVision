// Program.cs
using System;
using InputProcessor;
using UserManagement;
using LoggingService;

class Program
{
    static void Main()
    {
        Console.WriteLine("Starting Application...");

        // Dependency Injection to decouple DataHandler
        IDataProcessor dataHandler = new DataHandler();

        try
        {
            var processedData = dataHandler.ProcessData();
            Console.WriteLine($"Processed Data: {processedData}");

            // Move infinite loop to a separate process or thread if needed.  Remove for now.
            //dataHandler.InfiniteLoop();

            dataHandler.HardCodedValues();
            // Consider removing UnusedMethod or refactoring to provide value
            //dataHandler.UnusedMethod();
            dataHandler.InefficientStringConcatenation(); // This is slow!

            dataHandler.ExceptionSwallowing(); // Fix swallowing

            IUserManager userManager = new UserManager();
            var user = userManager.ManageUsers(); // This can throw an exception!
            Console.WriteLine($"User: {user}");

            ILogger logger = new Logger();
            logger.LogMessage($"Processed Data: {processedData} User: {user}");
        }
        catch (Exception ex)
        {
            // Centralized error handling
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Log the exception details to a file or logging service.  Don't just swallow it!
            ILogger logger = new Logger(); //Can use a static logger
            logger.LogMessage($"Critical error: {ex}");
        }
        finally
        {
            Console.WriteLine("Application finished.");
        }
    }
}


// Logger.cs
using System;
using System.IO;

namespace LoggingService
{
    public interface ILogger
    {
        void LogMessage(string message);
    }

    public class Logger : ILogger
    {
        private readonly string _logFilePath = "log.txt";

        public void LogMessage(string message)
        {
            try
            {
                // Use 'using' statement for automatic resource disposal
                using (StreamWriter writer = File.AppendText(_logFilePath))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}


// DataHandler.cs
using System;
using System.Text;

namespace InputProcessor
{
    public interface IDataProcessor
    {
        int ProcessData();
        void HardCodedValues();
        void InefficientStringConcatenation();
        void ExceptionSwallowing();
    }

    public class DataHandler : IDataProcessor
    {
        private int num1 = 10;
        private int num2 = 20;
        // Removing the large unused array
        // private int[] dataArray = new int[1000000]; // Unused memory allocation

        public int ProcessData()
        {
            Console.WriteLine("Processing Data...");
            // Reduce the loop count to something reasonable
            for (int i = 0; i < 100; i++)
            {
                // Console.WriteLine("Iteration: " + i); // Reduce console output for performance
            }
            int result = num1 + num2;
            Console.WriteLine("Calculated Result: " + result);
            return result;
        }

        // Removed InfiniteLoop as bad practice - if needed should be a separate thread.
        /*
        public void InfiniteLoop()
        {
            while (true)
            {
                Console.WriteLine("Running infinite loop...");
            }
        }
        */

        public void HardCodedValues()
        {
            const int value = 12345; // Hardcoded value, but making it a const to indicate its intent
            Console.WriteLine("Hardcoded value: " + value);
        }

        // Removed unused method

        public void InefficientStringConcatenation()
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < 10000; i++)
            {
                result.Append(i);
            }
            Console.WriteLine(result.ToString());
        }

        public void ExceptionSwallowing()
        {
            try
            {
                int x = 10;
                int y = 0;
                int result = x / y;
            }
            catch (Exception ex)
            {
                // Exception not swallowed - logging exception details!
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Log the exception to a file or logging service
                ILogger logger = new Logger(); //Or use a static logger
                logger.LogMessage($"Exception in ExceptionSwallowing: {ex}");

            }
        }
    }
}


// UserManager.cs
using System;

namespace UserManagement
{
    public interface IUserManager
    {
        string ManageUsers();
    }
    public class UserManager : IUserManager
    {
        public string ManageUsers()
        {
            string[] users = { "Alice", "Bob" };
            try
            {
                return users[1]; // Changed to return a valid user
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Index out of range: {ex.Message}");
                // Log the exception to a file or logging service.
                ILogger logger = new Logger(); // Or use a static logger
                logger.LogMessage($"Index out of range exception in UserManager: {ex}");
                return "DefaultUser"; // Return a default user instead of crashing.
            }
        }
    }
}