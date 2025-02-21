using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManagement
{
    public interface IUserManager
    {
        string GetUser(int index);
        IEnumerable<string> GetAllUsers();
    }

    public class UserManager : IUserManager
    {
        private readonly List<string> _users;

        public UserManager()
        {
            _users = new List<string> { "Alice", "Bob" };
        }

        public string GetUser(int index)
        {
            if (index < 0 || index >= _users.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of bounds");
            }
            return _users[index];
        }

        public IEnumerable<string> GetAllUsers()
        {
            return _users.AsReadOnly();
        }
    }
}