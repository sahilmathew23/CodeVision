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
        private readonly string _logFilePath;

        public Logger(string logFilePath = "log.txt")
        {
            _logFilePath = logFilePath;
        }

        public void LogMessage(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_logFilePath, true)) // Enables appending and ensures disposal
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch (IOException ex)
            {
                // Handling I/O errors that may occur when accessing the file
                Console.Error.WriteLine($"An error occurred while writing to the log: {ex.Message}");
                // Consider more sophisticated error reporting (e.g., event logging or another monitoring service)
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                Console.Error.WriteLine($"Unexpected error occurred: {ex.Message}");
            }
        }
    }
}