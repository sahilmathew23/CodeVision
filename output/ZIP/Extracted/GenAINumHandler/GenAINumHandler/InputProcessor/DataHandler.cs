using System;
using Microsoft.Extensions.Logging; // Requires Microsoft.Extensions.Logging NuGet package
using System.Diagnostics;

namespace InputProcessor
{
	public interface IDataProcessor
	{
		int ProcessData();
	}

	public class DataHandler : IDataProcessor
	{
		private readonly int _num1;
		private readonly int _num2;
		private readonly ILogger<DataHandler> _logger;

		public DataHandler( ILogger<DataHandler> logger, int num1 = 10, int num2 = 20 )
		{
			_logger = logger ?? throw new ArgumentNullException( nameof( logger ) ); // Guard against null logger
			_num1 = num1;
			_num2 = num2;
		}

		public int ProcessData()
		{
			_logger.LogInformation( "Data processing started." );
			var stopwatch = Stopwatch.StartNew();

			try
			{
				// Remove unnecessary loop and console writes
				//for ( int i = 0; i < 100000; i++ )
				//{
				//	Console.WriteLine( "Iteration: " + i );
				//}

				int result = CalculateResult( _num1, _num2 ); // Use a separate method for calculation
				_logger.LogInformation( "Calculated Result: {Result}", result );
				return result;
			}
			catch ( Exception ex )
			{
				_logger.LogError( ex, "An error occurred during data processing." );
				throw; // Re-throw the exception to allow the caller to handle it.  Consider custom exception type.
			}
			finally
			{
				stopwatch.Stop();
				_logger.LogInformation( "Data processing completed in {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds );
			}
		}

		private int CalculateResult( int a, int b )
		{
			_logger.LogDebug( "Calculating result with num1: {Num1} and num2: {Num2}", a, b );
			return a + b;
		}
	}
}


   using Microsoft.Extensions.Logging;

   public class Program
   {
       public static void Main(string[] args)
       {
           using var loggerFactory = LoggerFactory.Create(builder =>
           {
               builder.AddConsole(); // Log to console
               //builder.AddDebug();   // Log to debug output (VS)
           });

           ILogger<DataHandler> logger = loggerFactory.CreateLogger<DataHandler>();
           var handler = new InputProcessor.DataHandler(logger, 5, 7);
           int result = handler.ProcessData();

           Console.WriteLine($"Result: {result}");
       }
   }