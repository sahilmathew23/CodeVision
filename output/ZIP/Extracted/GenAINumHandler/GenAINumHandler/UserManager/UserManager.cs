using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UserManagement
{
    public interface IUserRepository
    {
        IEnumerable<string> GetUsernames();
    }

    public class UserRepository : IUserRepository
    {
        // Ideally, this would connect to a database.  For this example, using an in-memory list.
        private readonly List<string> _users = new List<string> { "Alice", "Bob" };

        public IEnumerable<string> GetUsernames()
        {
            // Defensive copy to prevent external modification of the internal list.
            return _users.ToList();
        }
    }


	public class UserManager
	{
		private readonly ILogger<UserManager> _logger;
        private readonly IUserRepository _userRepository;

        public UserManager(ILogger<UserManager> logger, IUserRepository userRepository)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		}

		public string ManageUsers(int userIndex)
		{
			try
			{
                var users = _userRepository.GetUsernames().ToArray();  // Get users from the repository
                if (userIndex >= 0 && userIndex < users.Length)
				{
					return users[userIndex];
				}
				else
				{
					_logger.LogError("Invalid user index: {userIndex}", userIndex);
					throw new ArgumentOutOfRangeException(nameof(userIndex), "User index is out of range.");
				}
			}
			catch (ArgumentOutOfRangeException ex)
			{
				_logger.LogError(ex, "Error managing users.");
				return "Error: Invalid User Index"; // Handle exception gracefully. Consider a custom exception type
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while managing users.");
                return "Error: An unexpected error occurred.";
            }
		}

        public string CreateUser(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogError("Username or password cannot be null or whitespace.");
                    throw new ArgumentException("Username and password must be provided.");
                }

                // Generate salt
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

                // Hash password with salt
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);

                // Combine salt and hash for storage (e.g., in a database)
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                string savedPasswordHash = Convert.ToBase64String(hashBytes);

                // In a real application, you would save the username and savedPasswordHash to a database.
                // This is a simplified example, so we're just logging the result.
                _logger.LogInformation("User created: {Username}", username);
                _logger.LogDebug("Password hash: {PasswordHash}", savedPasswordHash);

                return "User created successfully";
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return "Error: Invalid username or password";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a user.");
                return "Error: An unexpected error occurred during user creation.";
            }
        }
    }
}


// Example Usage (Program.cs or similar)
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using UserManagement;

public class Program
{
    public static void Main(string[] args)
    {
        // Setup dependency injection for logging.
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<IUserRepository, UserRepository>()
            .AddTransient<UserManager>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILogger<Program>>();
        var userManager = serviceProvider.GetService<UserManager>();



        try
        {
            string user = userManager.ManageUsers(0);
            Console.WriteLine($"User at index 0: {user}");

            string user2 = userManager.ManageUsers(1);
            Console.WriteLine($"User at index 1: {user2}");

            string user3 = userManager.ManageUsers(5); // Intentional out-of-range access
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred in Main.");
            Console.WriteLine("An error occurred. Check the logs.");
        }

         string creationResult = userManager.CreateUser("testuser", "P@$$wOrd");
         Console.WriteLine(creationResult);
    }
}