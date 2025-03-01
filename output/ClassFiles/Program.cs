using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NumberDetails
{
    // Interface for number analysis operations (Dependency Inversion Principle)
    public interface INumberAnalyzer
    {
        int FindLargest(List<int> numbers);
        int FindSmallest(List<int> numbers);
        double CalculateAverage(List<int> numbers);
    }

    // Concrete class implementing number analysis (Single Responsibility Principle)
    public class NumberAnalyzer : INumberAnalyzer
    {
        private readonly ILogger<NumberAnalyzer> _logger;

        public NumberAnalyzer(ILogger<NumberAnalyzer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int FindLargest(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                _logger.LogError("List of numbers is null or empty.");
                throw new ArgumentException("List of numbers cannot be null or empty.");
            }

            try
            {
                return numbers.Max(); // Use LINQ for improved performance and readability.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding the largest number.");
                throw; // Re-throw the exception after logging
            }
        }

        public int FindSmallest(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                _logger.LogError("List of numbers is null or empty.");
                throw new ArgumentException("List of numbers cannot be null or empty.");
            }

            try
            {
                return numbers.Min(); // Use LINQ for improved performance and readability.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding the smallest number.");
                throw; // Re-throw the exception after logging
            }
        }

        public double CalculateAverage(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                _logger.LogError("List of numbers is null or empty.");
                throw new ArgumentException("List of numbers cannot be null or empty.");
            }

            try
            {
                return numbers.Average(); // Use LINQ for improved performance and readability.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating the average.");
                throw; // Re-throw the exception after logging
            }
        }
    }


    public class Program
    {
        private static readonly ILogger<Program> _logger;
        private static readonly INumberAnalyzer _numberAnalyzer;

        // Static constructor for initialization (Dependency Injection emulation)
        static Program()
        {
            // Ideally, use a DI container for real-world scenarios
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole(); // or other logging providers
                builder.SetMinimumLevel(LogLevel.Information);
            });

            _logger = loggerFactory.CreateLogger<Program>();
            _numberAnalyzer = new NumberAnalyzer(loggerFactory.CreateLogger<NumberAnalyzer>()); // Inject logger
        }

        static void Main(string[] args)
        {
            _logger.LogInformation("Application started."); // Log application start

            Console.WriteLine("Welcome to the Number Details App!");

            List<int> numbers = new List<int> { 5, 10, 15, 20, 25 }; // Hardcoded list of numbers

            try
            {
                if (numbers.Count > 0)
                {
                    Console.WriteLine($"The largest number is: {_numberAnalyzer.FindLargest(numbers)}");
                    Console.WriteLine($"The smallest number is: {_numberAnalyzer.FindSmallest(numbers)}");
                    Console.WriteLine($"The average is: {_numberAnalyzer.CalculateAverage(numbers)}");
                }
                else
                {
                    Console.WriteLine("No numbers are available.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _logger.LogError(ex, "An argument exception occurred.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred.  Please check the logs.");
                _logger.LogError(ex, "An unexpected exception occurred.");
            }
            finally
            {
                Console.WriteLine("Thank you for using the app. Goodbye!");
                Console.ReadLine();
                _logger.LogInformation("Application ended."); // Log application end
            }
        }
    }
}