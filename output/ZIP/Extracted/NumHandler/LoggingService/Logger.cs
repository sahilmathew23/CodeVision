using System;
using System.IO;

namespace LoggingService
{
    public interface ILogger
    {
        void LogMessage(string message);
        void LogError(string message, Exception ex = null);
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
            Log(message, LogLevel.Information);
        }

        public void LogError(string message, Exception ex = null)
        {
            string errorMessage = ex != null ? $"{message} Exception: {ex}" : message;
            Log(errorMessage, LogLevel.Error);
        }

        private void Log(string message, LogLevel level)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(_logFilePath)) // Use 'using' for deterministic disposal
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] - {message}");
                }
            }
            catch (Exception ex)
            {
                //Handle logging exceptions to prevent application crash.
                Console.Error.WriteLine($"Error writing to log file: {ex.Message}");
            }

        }

        private enum LogLevel
        {
            Information,
            Error,
            Warning
        }
    }
}