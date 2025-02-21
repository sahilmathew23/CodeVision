Here is the enhanced version of `DataHandler.cs` with an explanation for each modification applied to meet the enhancement goals:

```csharp
using System;

namespace InputProcessor
{
    public interface IDataHandler
    {
        int ProcessData();
    }
    
    public class DataHandler : IDataHandler
    {
        private readonly ILogger _logger;
        private const int Num1 = 10;
        private const int Num2 = 20;

        public DataHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int ProcessData()
        {
            try
            {
                _logger.Log("Starting data processing...");

                int result = CalculateResult();

                _logger.Log($"Calculated Result: {result}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Log($"An error occurred: {ex.Message}");
                throw; // Rethrow the exception to higher layers potentially handling it further.
            }
        }

        private int CalculateResult()
        {
            return Num1 + Num2;
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
```

### Explanation of Modifications:

1. **Interface Extraction**: I introduced `IDataHandler` as an interface to adhere to the Dependency Inversion principle, a core aspect of SOLID, which also aids in making the code more modular and testable.

2. **Dependency Injection**: The `ILogger` interface was added for logging purposes, replacing direct calls to `Console.WriteLine`. This adheres to the Dependency Injection principle, allowing for greater flexibility and easier unit testing by passing in logger instances (e.g., you can pass a mocked logger in tests).

3. **Error Handling**: Robust error handling was added inside the `ProcessData()` method using a `try-catch` block to log errors and then rethrow them. This escalates handling to the caller, conforming to good error management practices.

4. **Code Clean-up**: Removed unused variable `dataArray` and excessive looping, improving the performance and reducing unnecessary memory usage and processing time.

5. **Const Usage**: Changed `num1` and `num2` to constants since these values do not change, which signals intent more clearly and offers slight performance benefits.

6. **Logging Improvement**: Use of an abstraction (`ILogger`) for logging rather than direct dependency on `Console.WriteLine` makes the module more reusable and testable. This step improves code quality by decoupling class dependencies.

7. **Single Responsibility Principle**: Splitting logging and data computation into separate responsibilities (via `ILogging` and distinct methods for computing results) adheres more closely to the Single Responsibility Principle.

### Conclusion
These enhancements make `DataHandler.cs` more maintainable, reusable, and easier to manage within a larger project context. It follows best practices such as SOLID principles, proper error handling, and improved performance which can significantly reduce scalability issues and optimize the system resources.