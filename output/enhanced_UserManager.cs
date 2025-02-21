### Enhanced Version of `UserManager.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManagement
{
    public interface IUserManager
    {
        string GetUserName(int userId);
        List<string> GetAllUserNames();
    }

    public class UserManager : IUserManager
    {
        private readonly List<string> users;

        public UserManager()
        {
            // Initializing with some sample users, would typically pull these from a database or a configuration file.
            users = new List<string> { "Alice", "Bob" };
        }

        public string GetUserName(int userId)
        {
            // Protecting against negative indices or indices larger than the list
            if (userId < 0 || userId >= users.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID is out of range");
            }
            return users[userId];
        }

        public List<string> GetAllUserNames()
        {
            // Returning a new copy of the list to preserve encapsulation
            return new List<string>(users);
        }
    }
}
```

### Explanation of Modifications:

1. **Interface `IUserManager`**:
   - Added an `IUserManager` interface to adhere to the Dependency Inversion Principle (part of SOLID). This allows for easier unit testing and future enhancements as other classes only depend on this interface rather than concrete implementations.
   
2. **Initialization of user list**:
   - Moved the user list initializations into the constructor to separate initialization from method logic and to allow for dynamic initialization strategies in the future (like fetching from a database).
   
3. **Exception Handling**:
   - Improved error handling in the `GetUserName` method by checking the index bounds and throwing an `ArgumentOutOfRangeException` if the index is out of range. This provides clear feedback and avoids the less informative `IndexOutOfRangeException` that would occur otherwise.
   
4. **Method Addition**:
   - Added `GetAllUserNames()` for modularity and improved functionality, returning a copy of the users list to ensure that external modifications don't affect the internal state, thus preserving encapsulation.
   
5. **Scalability and Performance**:
   - By using a `List<string>`, adding and removing users is more efficient (amortized O(1) for additions). If additional performance is required (e.g., frequent searching), consider using a different data structure or adding indexes.
   
6. **Security and Encapsulation**:
   - Ensure that the UserManager class handles user data responsibly. Here, only user names are held, and sensitive data management (if applicable) should always consider encryption and secure data handling practices.
   - Returning a new list in `GetAllUserNames()` instead of the actual list prevents callee from modifying the internal list state.
   
7. **.NET Coding Conventions**:
   - Followed conventions such as naming methods with verbs, and properties with nouns. Usage of `var` is avoided for clarity in the provided example, keeping explicit typing for educational purposes (though `var` is acceptable in practice).

These enhancements ensure that the `UserManager` class is more maintainable, scalable, reliable, and robust, aligning with professional coding practices and architectural principles.