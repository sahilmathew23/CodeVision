using System;
using System.Collections.Generic;

namespace ConsoleApp
{
	class Program
	{
		static void Main( string[] args )
		{
			Console.WriteLine( "Welcome to the Simple Console App!" );

			// Hardcoded list of numbers
			List<int> numbers = new List<int> { 5, 10, 15, 20, 25 };

			// Perform operations
			if ( numbers.Count > 0 )
			{
				Console.WriteLine( $"The largest number is: {FindLargest( numbers )}" );
				Console.WriteLine( $"The smallest number is: {FindSmallest( numbers )}" );
				Console.WriteLine( $"The average is: {CalculateAverage( numbers )}" );
			}
			else
			{
				Console.WriteLine( "No numbers are available." );
			}

			Console.WriteLine( "Thank you for using the app. Goodbye!" );
			Console.ReadLine();
		}

		static int FindLargest( List<int> numbers )
		{
			int largest = int.MinValue;
			foreach ( int num in numbers )
			{
				if ( num > largest )
					largest = num;
			}
			return largest;
		}

		static int FindSmallest( List<int> numbers )
		{
			int smallest = int.MaxValue;
			foreach ( int num in numbers )
			{
				if ( num < smallest )
					smallest = num;
			}
			return smallest;
		}

		static double CalculateAverage( List<int> numbers )
		{
			int sum = 0;
			foreach ( int num in numbers )
			{
				sum += num;
			}
			return ( double )sum / numbers.Count;
		}
	}
}
