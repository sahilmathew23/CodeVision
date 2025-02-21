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
            var dataHandler = new DataHandler();
            var processedData = dataHandler.ProcessData();

            var userManager = new UserManager();
            var user = userManager.ManageUsers();

            var logger = new Logger();
            logger.LogMessage($"Processed Data: {processedData} User: {user}");
        }
        catch (Exception ex)
        {
            var logger = new Logger();
            logger.LogMessage($"An error occurred: {ex.Message}", LogLevel.Error);
            Console.WriteLine("Application encountered an error. Please check logs.");
        }

        Console.WriteLine("Application ended.");
    }
}