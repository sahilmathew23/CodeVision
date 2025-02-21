using System;
using InputProcessor;
using UserManagement;
using LoggingService;

class Program
{
    private static readonly DataHandler _dataHandler = new DataHandler();
    private static readonly UserManager _userManager = new UserManager();
    private static readonly Logger _logger = new Logger();

    static void Main()
    {
        Console.WriteLine("Starting Application...");

        try
        {
            var processedData = _dataHandler.ProcessData();
            var user = _userManager.ManageUsers();
            
            _logger.LogMessage($"Processed Data: {processedData}, User: {user}");

            DoAdditionalProcessing(); // Additional processing that reuses the same instance of DataHandler
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during the application run: " + ex.Message);
        }
    }

    private static void DoAdditionalProcessing()
    {
        _dataHandler.ProcessData(); // Use the same DataHandler object
    }
}