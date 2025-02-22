using System;
using Microsoft.Extensions.DependencyInjection;
using InputProcessor;
using UserManagement;
using LoggingService;
using System.Threading.Tasks;

namespace MyApplication
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Application...");

            // Configure Dependency Injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IDataHandler, DataHandler>()
                .AddSingleton<IUserManager, UserManager>()
                .AddSingleton<ILogger, Logger>()
                .BuildServiceProvider();

            // Resolve dependencies
            var dataHandler = serviceProvider.GetService<IDataHandler>();
            var userManager = serviceProvider.GetService<IUserManager>();
            var logger = serviceProvider.GetService<ILogger>();

            try
            {
                // Process data asynchronously
                var processedData = await Task.Run(() => dataHandler.ProcessData());

                // Manage users asynchronously
                var user = await Task.Run(() => userManager.ManageUsers());

                // Log the results
                logger.LogMessage($"Processed Data: {processedData}, User: {user}");
            }
            catch (Exception ex)
            {
                // Centralized error handling
                logger.LogError($"An error occurred: {ex.Message}, StackTrace: {ex.StackTrace}");
                Console.Error.WriteLine($"Application encountered an error.  Check logs for details."); //Inform the user
                // Potentially handle the exception in a more sophisticated way, such as retrying or shutting down gracefully.
            }
            finally
            {
                Console.WriteLine("Application finished.");
            }

            // Optionally, dispose of the service provider if needed
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}


    namespace InputProcessor
    {
        public interface IDataHandler
        {
            string ProcessData();
        }
    }
    

    namespace UserManagement
    {
        public interface IUserManager
        {
            string ManageUsers();
        }
    }
    

    namespace LoggingService
    {
        public interface ILogger
        {
            void LogMessage(string message);
            void LogError(string message);
        }
    }
    

// InputProcessor/DataHandler.cs
using System;

namespace InputProcessor
{
    public class DataHandler : IDataHandler
    {
        public string ProcessData()
        {
            // Simulate data processing (replace with your actual logic)
            Console.WriteLine("Processing data...");
            System.Threading.Thread.Sleep(1000); // Simulate work
            return "Processed Data Result";
        }
    }
}

// UserManagement/UserManager.cs
using System;

namespace UserManagement
{
    public class UserManager : IUserManager
    {
        public string ManageUsers()
        {
            // Simulate user management (replace with your actual logic)
            Console.WriteLine("Managing users...");
            System.Threading.Thread.Sleep(500); // Simulate work
            return "User Management Result";
        }
    }
}

// LoggingService/Logger.cs
using System;

namespace LoggingService
{
    public class Logger : ILogger
    {
        public void LogMessage(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
            // Optionally, write to a file, database, or other logging system
        }
        public void LogError(string message)
        {
            Console.Error.WriteLine($"[ERROR] {DateTime.Now}: {message}");
            //Potentially log to a file, database, or logging service (like Serilog, NLog, etc.)
        }
    }
}