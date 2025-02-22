using System;
using Microsoft.Extensions.DependencyInjection;
using InputProcessor;
using UserManagement;
using LoggingService;

// Define an interface for the application's core functionality
public interface IApplication
{
    void Run();
}

// Implement the application logic using dependency injection
public class Application : IApplication
{
    private readonly IDataHandler _dataHandler;
    private readonly IUserManager _userManager;
    private readonly ILogger _logger;

    public Application(IDataHandler dataHandler, IUserManager userManager, ILogger logger)
    {
        _dataHandler = dataHandler ?? throw new ArgumentNullException(nameof(dataHandler));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Run()
    {
        _logger.LogMessage("Starting Application...");

        try
        {
            var processedData = _dataHandler.ProcessData();
            var user = _userManager.ManageUsers();

            _logger.LogMessage($"Processed Data: {processedData}, User: {user}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}", ex); //Enhanced error logging
        }

        _logger.LogMessage("Application finished.");
    }
}


class Program
{
    static void Main()
    {
        // Setup Dependency Injection
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IDataHandler, DataHandler>()
            .AddSingleton<IUserManager, UserManager>()
            .AddSingleton<ILogger, Logger>()
            .AddSingleton<IApplication, Application>() // Register the Application class
            .BuildServiceProvider();


        // Resolve the Application from the DI container
        var application = serviceProvider.GetService<IApplication>();

        // Run the application
        application.Run();
    }
}


// InputProcessor project
namespace InputProcessor
{
    public interface IDataHandler
    {
        string ProcessData();
    }

    public class DataHandler : IDataHandler
    {
        public string ProcessData()
        {
            return "Processed Data from DataHandler";
        }
    }
}

// UserManagement project
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
            return "User information from UserManager";
        }
    }
}

// LoggingService project
namespace LoggingService
{
    public interface ILogger
    {
        void LogMessage(string message);
        void LogError(string message, Exception ex);
    }

    public class Logger : ILogger
    {
        public void LogMessage(string message)
        {
            Console.WriteLine($"Log: {message}");
        }

        public void LogError(string message, Exception ex)
        {
            Console.Error.WriteLine($"Error: {message}\n{ex}");
        }
    }
}