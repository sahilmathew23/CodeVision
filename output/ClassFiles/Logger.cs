using System;
using System.IO;

namespace LoggingService
{
    public interface ILogger
    {
        void LogMessage(string message);
        void LogError(string message, Exception ex = null);
        void LogWarning(string message);
        void LogInformation(string message);
    }

    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _logFilePath;
        private StreamWriter _writer;
        private bool _disposed = false;


        public FileLogger(string logFilePath = "log.txt")
        {
            _logFilePath = logFilePath;
            try
            {
                // Append to the log file if it exists. Create if it doesn't.
                _writer = new StreamWriter(_logFilePath, true);
                _writer.AutoFlush = true; // Ensures data is written immediately.
                LogInformation("Logger initialized."); // Log initialization
            }
            catch (Exception ex)
            {
                // Handle the exception, potentially logging to a fallback logger or re-throwing
                Console.Error.WriteLine($"Error initializing FileLogger: {ex.Message}");
                throw; // Re-throw to indicate initialization failure
            }
        }

        public void LogMessage(string message)
        {
            LogInformation(message); // Default to information level
        }

        public void LogInformation(string message)
        {
            Log($"INFO: {message}");
        }

        public void LogWarning(string message)
        {
            Log($"WARN: {message}");
        }

        public void LogError(string message, Exception ex = null)
        {
            if (ex == null)
            {
                Log($"ERROR: {message}");
            }
            else
            {
                Log($"ERROR: {message} Exception: {ex}");
            }
        }

        private void Log(string message)
        {
            if (_disposed) return; // Prevent logging after disposal

            try
            {
                //Thread safety (simple locking)
                lock (_writer)
                {
                    _writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch (ObjectDisposedException)
            {
                Console.Error.WriteLine("Logger has been disposed.  Cannot write to log.");
            }
            catch (Exception ex)
            {
                //Handle logging failure gracefully
                Console.Error.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_writer != null)
                    {
                        _writer.Flush(); // Flush before closing
                        _writer.Close();
                        _writer.Dispose();
                        _writer = null;
                    }
                }

                // Dispose unmanaged resources, if any.

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        ~FileLogger()
        {
            // Finalizer to ensure resources are released even if Dispose isn't called.
            Dispose(disposing: false);
        }
    }
}


// Example Usage (Dependency Injection is recommended)
using (ILogger logger = new FileLogger("my_app.log"))
{
    try
    {
        // Some code that might throw an exception
        int result = 10 / 0; // This will throw a DivideByZeroException
    }
    catch (Exception ex)
    {
        logger.LogError("An error occurred during calculation.", ex);
    }

    logger.LogInformation("Application started.");
    logger.LogWarning("Low disk space detected.");
    logger.LogMessage("This is just a message.");  //Defaults to INFO level
}

//Outside the using block, the logger is automatically disposed of.