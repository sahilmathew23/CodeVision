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
        private readonly object _lockObject = new object();

        public Logger(string logFilePath = "log.txt")
        {
            _filePath = logFilePath;
            InitializeLog();
        }

        private void InitializeLog()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    using (StreamWriter writer = File.CreateText(_filePath))
                    {
                        writer.WriteLine("Logging initialized at " + DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize logging.", ex);
            }
        }

        public void LogMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message to log cannot be null or empty.");
            }

            try
            {
                lock (_lockObject)
                {
                    using (StreamWriter writer = new StreamWriter(_filePath, true))
                    {
                        writer.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
            }
            catch (IOException ex)
            {
                throw new IOException("Failed to write to log file.", ex);
            }
        }
    }
}