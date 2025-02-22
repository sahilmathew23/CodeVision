using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManagement
{
    // Interface for user repository, adhering to Dependency Inversion Principle (DIP)
    public interface IUserRepository
    {
        IEnumerable<string> GetUsers();
    }

    // Concrete implementation of UserRepository
    public class UserRepository : IUserRepository
    {
        public IEnumerable<string> GetUsers()
        {
            //Simulating data source access.  In a real scenario, this would access a database or other storage.
            return new List<string> { "Alice", "Bob" };
        }
    }


    public class UserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserManager> _logger;

        // Constructor injection for dependencies (DIP)
        public UserManager(IUserRepository userRepository, ILogger<UserManager> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Method to retrieve and process user list.  Error handling and logging implemented.
        public string ManageUsers()
        {
            try
            {
                var users = _userRepository.GetUsers().ToList(); // Get users from repository

                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found.");
                    return "No users found.";
                }


                // Example operation: Return the first user's name in uppercase.
                // This operation can be easily modified or extended without affecting other parts of the class.
                string firstUser = users.FirstOrDefault();
                if (string.IsNullOrEmpty(firstUser))
                {
                    _logger.LogWarning("User list is empty.");
                    return "User list is empty";
                }

                string result = firstUser.ToUpperInvariant();
                _logger.LogInformation("Successfully processed user: {User}", firstUser);
                return result;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while managing users."); // Log the exception with details
                return "An error occurred while managing users."; // Return a user-friendly message.  Consider using a custom exception type for better handling upstream.
            }
        }
    }
}


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UserManagement;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<UserManager>(); //No need to register logger - provided by framework
        // ... other services
    }
}



using Microsoft.AspNetCore.Mvc;
using UserManagement;

public class UserController : ControllerBase
{
    private readonly UserManager _userManager;

    public UserController(UserManager userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("/users")]
    public IActionResult GetUsers()
    {
        string result = _userManager.ManageUsers();
        return Ok(result);
    }
}