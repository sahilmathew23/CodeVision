using System;

namespace InputProcessor
{
	public class DataHandler
	{
		private int num1 = 10;
		private int num2 = 20;
		private int[] dataArray = new int[ 1000000 ]; // Unused memory allocation

		public int ProcessData()
		{
			Console.WriteLine( "Processing Data..." );
			for ( int i = 0; i < 100000; i++ ) // Unnecessary large loop
			{
				Console.WriteLine( "Iteration: " + i );
			}
			int result = num1 + num2;
			Console.WriteLine( "Calculated Result: " + result );
			return result;
		}

		public void InfiniteLoop()
		{
			while ( true ) // Bad practice: Infinite loop
			{
				Console.WriteLine( "Running infinite loop..." );
			}
		}

		public void HardCodedValues()
		{
			int value = 12345; // Hardcoded value
			Console.WriteLine( "Hardcoded value: " + value );
		}

		public void UnusedMethod()
		{
			int x = 5; // Unused variable
			int y = x * 2; // Unused computation
		}

		public void InefficientStringConcatenation()
		{
			string result = "";
			for ( int i = 0; i < 10000; i++ ) // Inefficient string concatenation
			{
				result += i;
			}
			Console.WriteLine( result );
		}

		public void ExceptionSwallowing()
		{
			try
			{
				int x = 10;
				int y = 0;
				int result = x / y;
			}
			catch ( Exception )
			{
				// Exception swallowed, no logging
			}
		}
	}


}