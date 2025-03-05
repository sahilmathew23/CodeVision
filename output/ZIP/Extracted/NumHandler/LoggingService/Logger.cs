namespace LoggingService
{
	public class Logger
	{
		public void LogMessage( string message )
		{
			System.IO.StreamWriter writer = new System.IO.StreamWriter( "log.txt" ); // No using statement, potential memory leak
			writer.WriteLine( "Logging started..." );
			writer.WriteLine( message );
			writer.Close();
		}
	}
}