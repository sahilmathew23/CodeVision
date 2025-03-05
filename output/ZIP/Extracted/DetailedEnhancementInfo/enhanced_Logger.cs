```csharp
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
```

**Explanation of Modifications:**

1.  **Interface `ILogger`:** Introduced an interface `ILogger` to define the logging contract. This promotes loose coupling and allows for easy swapping of different logging implementations in the future (Dependency Inversion Principle - SOLID).

2.  **Constructor with Dependency Injection:** The `Logger` class now has a constructor that takes the log file path as an argument. This enables configuring the log file path through dependency injection, making the logger more flexible and testable. The default is kept as "log.txt".

3.  **`using` statement for StreamWriter:** Enclosed the `StreamWriter` within a `using` statement. This ensures that the `StreamWriter` is properly disposed of after use, even if exceptions occur, preventing potential resource leaks.

4.  **Exception Handling in Logging:** Added a `try-catch` block around the file writing operation. This handles potential exceptions during logging (e.g., file access issues) and prevents the application from crashing.  The exception is written to Console.Error so the error is observable.

5.  **Log Levels:** Introduced `LogLevel` enum and updated the Log method. This helps to categorize log messages (Information, Error, Warning) and can be used for filtering logs in the future.

6.  **Error Logging Method:** Added `LogError` method to log error messages with associated exceptions. This provides more context for debugging and troubleshooting.

7.  **Date and Level in Logs:** Modified the logging format to include the current date and time and the log level, making logs easier to analyze.

8.  **Append to File:** Changed `new StreamWriter("log.txt")` to `File.AppendText("log.txt")` so each time a new log message is written to the file it doesn't overwrite the whole file, but rather appends to it.

**Benefits of these changes:**

*   **Improved Resource Management:** Using `using` ensures deterministic disposal of the `StreamWriter`, preventing resource leaks.
*   **Enhanced Error Handling:** The `try-catch` block prevents exceptions during logging from crashing the application and provides a fallback mechanism. The `LogError` adds useful exception information.
*   **Increased Flexibility:** The constructor allows configuring the log file path, making the logger more reusable in different contexts.
*   **Testability:** The interface and constructor injection improve the testability of the `Logger` class.
*   **SOLID Principles:** The design now adheres to the Dependency Inversion and Open/Closed Principles of SOLID.
*   **Better Logging Information:**  Log levels and timestamps make logs more informative for debugging and monitoring.

This enhanced `Logger.cs` addresses the identified issues and improves the overall quality, maintainability, and reliability of the logging service.
