using System;
using System.Collections.Generic;
using System.Linq; // For LINQ operations
using Microsoft.Extensions.Logging; // Use Microsoft.Extensions.Logging
using Microsoft.Extensions.DependencyInjection; // Use dependency injection

namespace ConsoleApp
{
    // Interface for number operations
    public interface INumberProcessor
    {
        int FindLargest(List<int> numbers);
        int FindSmallest(List<int> numbers);
        double CalculateAverage(List<int> numbers);
    }

    // Implementation of number operations
    public class NumberProcessor : INumberProcessor
    {
        private readonly ILogger<NumberProcessor> _logger;

        public NumberProcessor(ILogger<NumberProcessor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int FindLargest(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                _logger.LogError("The list of numbers is null or empty when trying to find the largest number.");
                throw new ArgumentException("List of numbers cannot be null or empty.");
            }

            try
            {
                return numbers.Max(); // Use LINQ for better performance
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while finding the largest number.");
                throw; // Re-throw the exception for handling upstream
            }
        }

        public int FindSmallest(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                _logger.LogError("The list of numbers is null or empty when trying to find the smallest number.");
                throw new ArgumentException("List of numbers cannot be null or empty.");
            }

            try
            {
                return numbers.Min(); // Use LINQ for better performance
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while finding the smallest number.");
                throw; // Re-throw the exception for handling upstream
            }
        }

        public double CalculateAverage(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                _logger.LogError("The list of numbers is null or empty when trying to calculate the average.");
                throw new ArgumentException("List of numbers cannot be null or empty.");
            }

            try
            {
                return numbers.Average(); // Use LINQ for better performance
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating the average.");
                throw; // Re-throw the exception for handling upstream
            }
        }
    }


	class Program
	{
		static void Main( string[] args )
		{
            // Setup dependency injection
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole(); // Add console logger.  Can be configured further
                    // Add other logging providers like file, database, etc. here
                })
                .AddSingleton<INumberProcessor, NumberProcessor>()
                .BuildServiceProvider();

            // Resolve the INumberProcessor
            var numberProcessor = serviceProvider.GetService<INumberProcessor>();
            var logger = serviceProvider.GetService<ILogger<Program>>();

			Console.WriteLine( "Welcome to the Simple Console App!" );

			// Hardcoded list of numbers
			List<int> numbers = new List<int> { 5, 10, 15, 20, 25 };

			// Perform operations
			if ( numbers.Count > 0 )
			{
                try
                {
                    Console.WriteLine($"The largest number is: {numberProcessor.FindLargest(numbers)}");
                    Console.WriteLine($"The smallest number is: {numberProcessor.FindSmallest(numbers)}");
                    Console.WriteLine($"The average is: {numberProcessor.CalculateAverage(numbers)}");
                }
                catch (ArgumentException ex)
                {
                    logger.LogError(ex, "An error occurred while processing the numbers.");
                    Console.WriteLine("An error occurred while processing the numbers. Please check the logs for more details.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An unexpected error occurred.");
                    Console.WriteLine("An unexpected error occurred. Please check the logs for more details.");
                }

			}
			else
			{
				Console.WriteLine( "No numbers are available." );
			}

			Console.WriteLine( "Thank you for using the app. Goodbye!" );
			Console.ReadLine();
		}

	}
}