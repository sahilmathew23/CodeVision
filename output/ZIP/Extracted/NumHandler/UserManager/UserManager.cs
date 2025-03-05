using Microsoft.Extensions.Logging; // Requires Microsoft.Extensions.Logging NuGet package

namespace UserManagement
{
	public interface IUserManager
	{
		string ManageUsers();
	}

	public class UserManager : IUserManager
	{
		private readonly ILogger<UserManager> _logger;

		public UserManager( ILogger<UserManager> logger )
		{
			_logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
		}

		public string ManageUsers()
		{
			string[] users = { "Alice", "Bob" };
			int userIndex = 1; // Safely access an existing user

			if ( userIndex >= 0 && userIndex < users.Length )
			{
				_logger.LogInformation( "User '{User}' selected.", users[ userIndex ] );
				return users[ userIndex ];
			}
			else
			{
				_logger.LogError( "Invalid user index: {UserIndex}", userIndex );
				throw new IndexOutOfRangeException( $"User index {userIndex} is out of range." );
			}
		}
	}
}


using Microsoft.Extensions.DependencyInjection; // Requires Microsoft.Extensions.DependencyInjection NuGet package
using Microsoft.Extensions.Logging;
using InputProcessor;
using UserManagement;
using LoggingService;
using Microsoft.Extensions.Hosting;

class Program
{
	static void Main()
	{
		Console.WriteLine( "Starting Application..." );

		// Setup dependency injection
		using IHost host = Host.CreateDefaultBuilder()
			.ConfigureServices( ( _, services ) =>
			{
				services.AddTransient<IUserManager, UserManager>(); //Register the user manager.
				services.AddLogging( builder =>
				{
					builder.AddConsole(); // Configure console logging
					builder.AddDebug();   // Configure debug logging (Visual Studio output)
					builder.AddFile( "app.log", append: true );
				} );
				services.AddTransient<DataHandler>();
				services.AddTransient<LoggingService.Logger>();

			} )
			.Build();

		var dataHandler = host.Services.GetRequiredService<DataHandler>();
		var userManager = host.Services.GetRequiredService<IUserManager>();
		var logger = host.Services.GetRequiredService<LoggingService.Logger>();
		var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();


		var processedData = dataHandler.ProcessData();

		try
		{
			var user = userManager.ManageUsers();
			logger.LogMessage( "Processed Data: " + processedData + " User: " + user );
		}
		catch ( IndexOutOfRangeException ex )
		{
			Console.WriteLine( $"An error occurred: {ex.Message}" );
			// Handle the exception appropriately (e.g., log it, display an error message to the user).
			loggerFactory.CreateLogger( "Program" ).LogError( ex, "Error managing users" );

		}

		//Call other DataHandler methods
		//dataHandler.InfiniteLoop(); // Introduces infinite loop
		dataHandler.HardCodedValues(); // Uses hardcoded values
		dataHandler.UnusedMethod(); // Calls an unused method
		dataHandler.InefficientStringConcatenation(); // Causes performance issues
		dataHandler.ExceptionSwallowing(); // Swallows exceptions

		Console.WriteLine( "Application Finished..." );

	}
}

// Extension method for adding file logging.  Requires `Serilog.Sinks.File` NuGet package
public static class LoggingBuilderExtensions
{
	public static ILoggingBuilder AddFile( this ILoggingBuilder builder, string filePath, bool append = false )
	{
		builder.AddSerilog( new LoggerConfiguration()
			.WriteTo.File( filePath, appendToExisting: append )
			.CreateLogger() );

		return builder;
	}
}