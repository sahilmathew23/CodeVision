using System;
using InputProcessor;
using UserManagement;
using LoggingService;

class Program
{
    static void Main()
    {
        Console.WriteLine("Starting Application...");

        try
        {
            IDataHandler dataHandler = new DataHandler();
            var processedData = SafeExecute(() => dataHandler.ProcessData(), "Process Data");

            IUserManager userManager = new UserManager();
            var user = SafeExecute(() => userManager.ManageUsers(), "Manage Users");

            ILogger logger = new Logger();
            logger.LogMessage($"Processed Data: {processedData} User: {user}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    private static T SafeExecute<T>(Func<T> func, string operation)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error during {operation}: {ex.Message}");
            throw;
        }
    }
}