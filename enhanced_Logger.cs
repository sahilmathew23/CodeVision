### Enhanced Version of `Logger.cs`:
```csharp
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
                // Using "append" mode to avoid overwriting existing logs
                using (StreamWriter writer = new StreamWriter(_logFilePath, append: true))
                {
                    writer.WriteLine($"Log Entry : {DateTime.Now}");
                    writer.WriteLine(message);
                    writer.WriteLine("-------------------------------");
                }
            }
            catch (Exception ex)
            {
                // Basic exception handling; could log to a secondary source or raise further
                Console.Error.WriteLine($"Failed to log message: {ex}");
            }
        }
    }
}
```

### Explanation of Modifications:
1. **Interface Implementation (`ILogger`):**
   - Introduced an `ILogger` interface to follow the Dependency Inversion Principle ("D" in SOLID), allowing for easier replacement or extension of logging functionality without affecting dependent classes.

2. **Flexible File Path:**
   - Constructor injection is used to optionally specify a log file path, increasing flexibility and ease of configuration.
   - Default value of "log.txt" is retained so existing code behaves the same if not updated.

3. **Using Statement:**
   - Wrapped the `StreamWriter` object in a `using` statement to ensure the cleanup of resources (`IDisposable` pattern). This prevents file handle leaks and ensures files are closed properly after writing.

4. **Exception Handling:**
   - Added basic exception handling around the file I/O operations to prevent the application from crashing due to IO exceptions like file access permissions errors. This logs the error to the console, but could easily be extended to include more sophisticated log failover scenarios (like logging to a secondary source).

5. **Append Mode for StreamWriter:**
   - Set the `StreamWriter` to append mode to prevent overwriting existing logs, making it suitable for ongoing logging across application sessions.

6. **Improved Log Formatting:**
   - Included timestamps and separators (`-------------------------------`) in log entries to enhance the readability and usefulness of the recorded logs.

7. **Security and Performance Considerations:**
   - Using `StreamWriter` directly is typically secure from injection attacks since writing is done to a file and not executed as code. However, file path handling and ensuring exclusive file write access could be further addressed depending on deployment specifics.
   - For improved performance in multi-threaded scenarios, or where the log file might become a bottleneck, further enhancements could involve asynchronous logging mechanisms or buffered loggers.

This enhanced version adheres to .NET coding conventions, leverages SOLID principles for maintainability and future extensibility, and integrates basic best practices for error handling and performance.