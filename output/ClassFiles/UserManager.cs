using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManagement
{
    public interface IUserManager
    {
        string GetUserName(int index);
        IList<string> GetAllUsers();
    }

    public class UserManager : IUserManager
    {
        private readonly List<string> _users;

        public UserManager()
        {
            _users = new List<string> { "Alice", "Bob" };
        }

        public string GetUserName(int index)
        {
            if (index < 0 || index >= _users.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"User index {index} is out of bounds.");
            }
            return _users[index];
        }

        public IList<string> GetAllUsers() => _users.AsReadOnly();

        public void AddUser(string user)
        {
            // Consider more validations or business rules as necessary
            if (string.IsNullOrWhiteSpace(user))
            {
                throw new ArgumentException("Invalid user name: User name cannot be null or whitespace.");
            }

            if (_users.Contains(user))
            {
                throw new InvalidOperationException("User already exists.");
            }

            _users.Add(user);
        }
    }
}