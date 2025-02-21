using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManagement
{
    public interface IUserManager
    {
        string FetchUser(int userId);
        IEnumerable<string> FetchAllUsers();
    }

    public class UserManager : IUserManager
    {
        private List<string> _users;

        public UserManager()
        {
            _users = new List<string> { "Alice", "Bob" }; // Initialize with default users
        }

        public string FetchUser(int userId)
        {
            if (userId < 0 || userId >= _users.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID is out of range.");
            }

            return _users[userId];
        }

        public IEnumerable<string> FetchAllUsers()
        {
            return _users.AsReadOnly();
        }
    }
}