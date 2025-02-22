using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace LoggingService
{
    public interface IAppLogger
    {
        void LogInformation(string message);
        void LogWarning(string message);
        LogError(string message, Exception ex = null);
    }

    public class AppLogger : IAppLogger
    {
        private readonly ILogger _logger;
        private readonly string _logFilePath;

        public AppLogger(ILogger<AppLogger> logger, string logFilePath = "app.log") // Inject ILogger and allow configurable log file path
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath)); ; // Default log file path
            EnsureLogFileExists();
        }

        private void EnsureLogFileExists()
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    // Create the file if it doesn't exist
                    using (File.Create(_logFilePath)) { } // Create and immediately dispose to release the handle.
                }
            }
            catch (Exception ex)
            {
                //If the log file can't be created, log it to the Console
                Console.WriteLine($"Error creating log file: {ex.Message}");
                Console.Error.WriteLine($"Error creating log file: {ex.Message}");
            }
        }


        public void LogInformation(string message)
        {
            Log(LogLevel.Information, message);
        }

        public void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public void LogError(string message, Exception ex = null)
        {
            Log(LogLevel.Error, message, ex);
        }

        private void Log(LogLevel logLevel, string message, Exception ex = null)
        {
            try
            {
                string logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}";

                if (ex != null)
                {
                    logEntry += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
                }

                _logger.Log(logLevel, logEntry); //Use the injected logger

                //Append to file.  Uses a try/catch to prevent crashes if the log file can't be written to for any reason
                try
                {
                    using (var writer = File.AppendText(_logFilePath))
                    {
                        writer.WriteLine(logEntry);
                    }
                }
                catch (Exception fileEx)
                {
                    _logger.LogError($"Error writing to log file: {fileEx.Message}");
                    // Fallback: If writing to the log file fails, write to console.
                    Console.WriteLine($"Error writing to log file, falling back to console: {fileEx.Message}");
                    Console.Error.WriteLine(logEntry);
                }
            }
            catch (Exception overallEx)
            {
                //In the very rare event that logging fails completely, log it to the console
                Console.WriteLine($"FATAL: Logging failed: {overallEx.Message}");
                Console.Error.WriteLine($"FATAL: Logging failed: {overallEx.Message}");
            }

        }
    }
}


    builder.Services.AddSingleton<IAppLogger, AppLogger>();
    builder.Services.AddLogging(configure => configure.AddConsole()); // Adds console logger
    builder.Services.AddSingleton<ILogger<AppLogger>>(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger<AppLogger>());
    

    public class MyService
    {
        private readonly IAppLogger _logger;

        public MyService(IAppLogger logger)
        {
            _logger = logger;
        }

        public void DoSomething()
        {
            try
            {
                // ... some code that might throw an exception ...
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while doing something.", ex);
            }
        }
    }