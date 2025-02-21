### Enhanced Version of `UserManager.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManagement
{
    public interface IUserManager
    {
        string GetUser(int userId);
        IEnumerable<string> GetAllUsers();
    }
    
    public class UserManager : IUserManager
    {
        private readonly IList<string> _users;

        public UserManager()
        {
            _users = new List<string> { "Alice", "Bob" }; 
        }

        public string GetUser(int userId)
        {
            if (userId < 0 || userId >= _users.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID is out of range.");
            }
            return _users[userId];
        }

        public IEnumerable<string> GetAllUsers()
        {
            return _users.ToList();
        }
    }
}
```

### Explanation of Modifications

1. **Implementation of Interface (IUserManager)**:
   - To adhere to the **Dependency Inversion** and **Interface Segregation** principles of SOLID, I introduced an `IUserManager` interface. This allows the high-level modules not to depend directly on the concrete implementation of the `UserManager` and makes it easier to mock or replace in tests or other implementations.

2. **Refactoring User Storage**:
   - Changed `users` array to an `IList<string>` to allow for more dynamic management of users (adding/removing not shown here, but possible with this structure). This improves the scalability and maintainability of the user list.

3. **Error Handling**:
   - Added an explicit check and `ArgumentOutOfRangeException` in the `GetUser` method to handle invalid user indices robustly. This strengthens error handling by providing clear feedback on what went wrong.

4. **Modularity and Reusability**:
   - By implementing `GetUser` and `GetAllUsers` methods and using an interface, other parts of the application can reuse and extend `UserManager` functionality without modifying its internal implementation.

5. **Security Best Practices**:
   - Though specific security measures depend heavily on context (e.g., user data encryption, secure API design), validating inputs and mitigating exceptions are fundamental practices applied here. 

6. **.NET Coding Conventions**:
   - Maintained consistent brace style, naming conventions, and method encapsulation based on common .NET conventions.

This refactored version of `UserManager` enhances its maintainability, scalability, and robustness, better aligning with professional coding standards and practices.