using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace InputProcessor
{
	// Interface for data processing to adhere to the Dependency Inversion Principle.
	public interface IDataProcessor
	{
		int ProcessData();
	}

	// DataProcessor class implementing the interface, handles the core data processing logic.
	public class DataProcessor : IDataProcessor
	{
		private readonly int _num1 = 10;
		private readonly int _num2 = 20;
		private readonly ILogger<DataProcessor> _logger;

		// Constructor injection for dependencies like ILogger.  This promotes testability and loose coupling.
		public DataProcessor( ILogger<DataProcessor> logger )
		{
			_logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
		}

		public int ProcessData()
		{
			_logger.LogInformation( "Starting data processing." );

			// Use Stopwatch for performance monitoring.
			var stopwatch = Stopwatch.StartNew();

			// Removed unnecessary large loop for performance.
			//_logger.LogDebug("Executing loop iterations..."); // Removed as unnecessary
			//for (int i = 0; i < 100000; i++)
			//{
			//    // Emulate some processing if needed, but avoid excessive console output.
			//    if (i % 10000 == 0) // Log every 10000 iterations to avoid excessive output
			//    {
			//        _logger.LogTrace("Iteration: {IterationNumber}", i);
			//    }
			//}

			int result;
			try
			{
				result = CalculateResult();
				_logger.LogInformation( "Calculation successful." );
			}
			catch ( Exception ex )
			{
				// Centralized error handling with logging.
				_logger.LogError( ex, "An error occurred during data processing." );
				throw; // Re-throw the exception to allow the calling code to handle it.  Consider a custom exception here.
			}
			finally
			{
				stopwatch.Stop();
				_logger.LogInformation( "Data processing completed in {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds );
			}

			return result;
		}

		// Separated the calculation into its own method for better Single Responsibility.
		private int CalculateResult()
		{
			_logger.LogDebug( "Calculating result..." );
			int result = _num1 + _num2;
			_logger.LogDebug( "Calculated result: {Result}", result );
			return result;
		}
	}

	// Wrapper to create a new scope for data processing and handle dependencies.
	public class DataHandler
	{
		private readonly IDataProcessor _dataProcessor;

		public DataHandler( IDataProcessor dataProcessor )
		{
			_dataProcessor = dataProcessor ?? throw new ArgumentNullException( nameof( dataProcessor ) );
		}

		public int HandleData()
		{
			Console.WriteLine( "Starting Data Handling..." );
			int result = _dataProcessor.ProcessData();
			Console.WriteLine( "Data Handling Completed." );
			return result;
		}
	}
}


// In Startup.cs (ConfigureServices method)

public void ConfigureServices( IServiceCollection services )
{
	// Add logging
	services.AddLogging( builder =>
	{
		builder.AddConsole(); // Or any other logging provider (e.g., ApplicationInsights)
		builder.AddDebug();
	} );

	// Register dependencies
	services.AddScoped<IDataProcessor, DataProcessor>();
	services.AddScoped<DataHandler>();
}


// Resolve DataHandler from the DI container
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ...

// Create service collection
var services = new ServiceCollection();

// Configure services (as shown in the previous example)
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

services.AddScoped<IDataProcessor, DataProcessor>();
services.AddScoped<DataHandler>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Resolve DataHandler
var dataHandler = serviceProvider.GetService<DataHandler>();

if (dataHandler != null)
{
    int result = dataHandler.HandleData();
    Console.WriteLine("Result: " + result);
}
else
{
    Console.WriteLine("Failed to resolve DataHandler from the DI container.");
}