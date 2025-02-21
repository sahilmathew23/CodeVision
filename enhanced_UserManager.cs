Here's the enhanced version of the `UserManager.cs` file, incorporating best practices:

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace UserManagement
{
    public interface IUserManager
    {
        string GetUserById(int id);
        List<string> GetAllUsers();
    }

    public class UserManager : IUserManager
    {
        private readonly ILogger<UserManager> _logger;
        private readonly List<string> _users;

        public UserManager(ILogger<UserManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _users = new List<string> { "Alice", "Bob" };
            _logger.LogDebug("UserManager initialized with {UserCount} users.", _users.Count);
        }

        public string GetUserById(int id)
        {
            if (id < 0 || id >= _users.Count)
            {
                _logger.LogError("Attempted to access Invalid user index: {Index}", id);
                return null; // or throw an appropriate exception based on your error handling policy
            }

            _logger.LogInformation("User retrieved: {User}", _users[id]);
            return _users[id];
        }

        public List<string> GetAllUsers()
        {
            _logger.LogInformation("Retrieving all users, total count: {Count}", _users.Count);
            return new List<string>(_users);
        }
    }
}
```

### Explanation of Modifications:

1. **Separation of Concerns & Interface Segregation:**
   - An `IUserManager` interface was introduced to define the operations cleanly. This promotes modularity and ensures that future extensions or testing can be done without altering the actual implementation.
   
2. **Logging and Error Handling:**
   - Integrated `ILogger` for logging important information and errors, which aids in debugging and traceability. Using structured logging (e.g., with parameters such as user count and index), helps in maintaining readability and effectiveness of logs.
   - Throwing an exception if a logger is not provided enforces that logging is considered a mandatory dependency.
   - Fine-grained and informative error messages provide insight into failures like trying to access an invalid index.

3. **Dependency Injection:**
   - The `ILogger<UserManager>` dependency is injected via the constructor, following the Dependency Injection pattern, which makes the class less dependent on specific logger configurations and easier to manage or test.

4. **Proper Exception Handling & Security:**
   - Instead of allowing an index out-of-range exception to occur (and potentially leak sensitive information or disrupt service), checks are performed to validate indices. This improves security by preventing accidental exposure of internal workings (like storage details or memory structure).
   - Returns `null` safely if an invalid index is accessed. Depending on application policy, you might choose to throw a custom, more descriptive exception here.

5. **Performance and Modularity:**
   - Avoids using raw arrays for user storage and instead utilizes `List<string>`, which provides more flexibility for future operations like adding or removing users.
   - The `GetAllUsers` method creates a new list from the stored users, ensuring that the caller can't modify the original list, which encapsulates the data safely.

6. **.NET Coding Conventions:**
   - Following .NET naming and error-handling conventions helps in maintaining clean and maintainable code. The use of `var` keyword has been avoided for clarity in this context. Naming and method functionalities are kept intuitive and explicit.

This enhanced `UserManager` class now is more robust, scalable, and easier to maintain. It fits well into a larger project architecture with clear opportunities for future enhancements without broad refactoring.