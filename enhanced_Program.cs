The enhanced version of `Program.cs` would consider each of the given goals for improvement. Below is the refactored version of the program with detailed explanations afterward.

```csharp
using System;
using InputProcessor;
using UserManagement;
using LoggingService;
using System.Diagnostics;

class Program
{
    // Declare dependencies as static fields to avoid redundant object creation
    private static DataHandler dataHandler = new DataHandler();
    private static UserManager userManager = new UserManager();
    private static Logger logger = new Logger();
    
    static void Main()
    {
        try
        {
            Console.WriteLine("Starting Application...");

            var processedData = ProcessData();
            var user = ManageUsers();
            LogDetails(processedData, user);

            Console.WriteLine("Application Finished Successfully.");
        }
        catch (Exception ex)
        {
            // Log exception details, potentially with more comprehensive information in a full application
            logger.LogMessage("An error occurred: " + ex.Message);
            Debug.WriteLine(ex); // For development debugging purposes

            // It could be advisable to also log the stack trace or handle different types of exceptions differently.
            Environment.Exit(1); // Exit the application due to an error after logging it
        }
    }

    private static string ProcessData()
    {
        // Moved redundant object creation
        return dataHandler.ProcessData();
    }

    private static User ManageUsers()
    {
        return userManager.ManageUsers();
    }

    private static void LogDetails(string data, User user)
    {
        var message = $"Processed Data: {data} User: {user}";
        logger.LogMessage(message);
    }
}
```

### Explanation of Changes:

1. **Refactoring SOLID Principles**:
   - **Single Responsibility Principle**: Functions within 'Main' were extracted to separate methods (`ProcessData`, `ManageUsers`, `LogDetails`), ensuring that each function does one thing only.
   - **Dependency Inversion Principle**: Rather than creating instances inside methods, dependencies are now at the class level, reducing tight coupling. In a more advanced scenario, dependence on interfaces rather than concrete implementations would be ideal.

2. **Modularity and Reusability**:
   - By moving operations into separate methods, each operation can now be reused or modified independently in future enhancements.

3. **Improve Performance and Scalability**:
   - By declaring `dataHandler`, `userManager`, and `logger` as static fields, redundant instantiations are eliminated, which reduces resource consumption and enhances performance.

4. **Strengthen Error Handling and Logging**:
   - Implemented a `try-catch` block to handle exceptions gracefully and log them correctly. This prevents the application from crashing by unexpected exceptions and ensures all errors are logged for maintenance and audits.

5. **Security Best Practices**:
   - Logging sensitive information directly or handling exceptions poorly can expose sensitive data or internal mechanisms. Here, exceptions are logged with caution, and consideration for what is logged. For further enhancements, ensure no sensitive data makes it to logs.
   - Application now gracefully shuts down (`Environment.Exit(1)`) on encountering an error, a practice that can prevent further error cascading on exception.

6. **.NET Coding Conventions**:
   - Maintained C# naming and style conventions, usage of `var` where appropriate, and proper spacing and line usage for readability.

This refactoring enhances the maintainability and clarity of the original script while ensuring performance, modularity, and safety enhancements suitable for a growing project.