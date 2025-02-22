```csharp
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
```

**Explanation of Modifications:**

1.  **Interface `IAppLogger`:** Introduced an interface `IAppLogger` to define the contract for logging operations. This adheres to the Dependency Inversion Principle (DIP) and allows for easy swapping of logging implementations.  It promotes loose coupling.

2.  **Class `AppLogger`:** Implements the `IAppLogger` interface.  This is the concrete implementation of our logger.

3.  **Dependency Injection:** The `AppLogger` constructor now accepts an `ILogger<AppLogger>` instance via dependency injection.  This uses the built-in .NET logging framework which is much more robust than a simple `StreamWriter`.  It adheres to DIP and promotes testability.  Also, it takes in an optional `logFilePath` so we can configure where the log files get stored.

4.  **`Microsoft.Extensions.Logging`:** Leverages the built-in .NET logging infrastructure for more advanced logging features (log levels, categories, etc.).  This replaces the simple `Console.WriteLine` approach with more structured logging.

5.  **Log Levels:** Introduced `LogInformation`, `LogWarning`, and `LogError` methods, each mapping to a specific log level defined in `Microsoft.Extensions.Logging.LogLevel`. This allows for filtering logs based on severity.

6.  **Exception Handling:** The `LogError` method now accepts an optional `Exception` object, allowing you to log exception details along with the message.  The logging mechanism handles `Exception` objects now.

7.  **`using` statement (or equivalent):**  Replaced `writer.Close()` with a `using` statement around the `StreamWriter`.  This ensures that the file stream is properly disposed of, even if exceptions occur, preventing potential file locking issues and resource leaks. The `EnsureLogFileExists()` method uses `using` statement too, to ensure the file handle is released.

8.  **Configurable Log File Path:** The log file path is now configurable via the constructor, allowing for greater flexibility.

9.  **Error Handling around File Operations:** Added comprehensive `try-catch` blocks around file creation and writing operations. This prevents application crashes if the log file is inaccessible or if there are permission issues.  Error messages are logged to the console as a fallback.

10. **Thread Safety (Consideration):**  While the provided code doesn't explicitly handle multi-threading, the use of `File.AppendText` within a `using` statement provides some level of thread safety for appending to the log file. However, for high-volume, multi-threaded scenarios, a dedicated logging library with built-in thread safety mechanisms (e.g., `ConcurrentQueue` and a background thread) is recommended.

11. **Date/Time Stamp:** Added a date/time stamp to each log entry for better traceability.

12. **SOLID Principles:**
    *   **Single Responsibility Principle (SRP):**  The `AppLogger` class focuses solely on logging.  The logic responsible for file creation is separated into `EnsureLogFileExists`
    *   **Open/Closed Principle (OCP):** The `AppLogger` can be extended with new logging functionalities without modifying its core implementation.
    *   **Liskov Substitution Principle (LSP):**  `AppLogger` can be substituted with any other class that implements `IAppLogger` without affecting the correctness of the program.
    *   **Interface Segregation Principle (ISP):** The `IAppLogger` interface is specific to logging operations.
    *   **Dependency Inversion Principle (DIP):** The `AppLogger` depends on abstractions (`ILogger`, `IAppLogger`) rather than concrete implementations.

13. **Maintain .NET Coding Conventions:** Code is formatted according to standard .NET conventions (PascalCase for class and method names, camelCase for variables, etc.).

14. **Log file creation:** A method to create a log file is added to create a log file only if it doesn't already exist.

15. **Fallback to console:** Adds fallback to the console output stream should the log files be inaccessible for some reason.

**How to Use:**

1.  **Dependency Injection:**  Register `IAppLogger` and `ILogger` in your dependency injection container.  For example, in a .NET Core application:

    ```csharp
    builder.Services.AddSingleton<IAppLogger, AppLogger>();
    builder.Services.AddLogging(configure => configure.AddConsole()); // Adds console logger
    builder.Services.AddSingleton<ILogger<AppLogger>>(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger<AppLogger>());
    ```

2.  **Inject `IAppLogger`:** Inject `IAppLogger` into the classes where you need to log messages.

    ```csharp
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
    ```

This revised implementation addresses the issues of the original code, provides a more robust and flexible logging solution, and adheres to best practices and SOLID principles. It leverages the standard .NET logging framework and provides clear separation of concerns.
