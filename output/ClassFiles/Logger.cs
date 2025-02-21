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
        private readonly string _filePath;

        public Logger(string logFilePath = "log.txt")
        {
            _filePath = logFilePath;
            EnsureLogDirectoryExists();
        }

        public void LogMessage(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_filePath, true))
                {
                    writer.WriteLine($"{DateTime.UtcNow}: {message}");
                }
            }
            catch (Exception ex)
            {
                // Here, consider logging to an alternative location or triggering an alert
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }
        }

        private void EnsureLogDirectoryExists()
        {
            string directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}