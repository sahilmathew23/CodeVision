using System;
using InputProcessor;
using UserManagement;
using LoggingService;

class Program
{
	static void Main()
	{
		Console.WriteLine( "Starting Application..." );

		DataHandler dataHandler = new DataHandler();
		var processedData = dataHandler.ProcessData();
		dataHandler.InfiniteLoop(); // Introduces infinite loop
		dataHandler.HardCodedValues(); // Uses hardcoded values
		dataHandler.UnusedMethod(); // Calls an unused method
		dataHandler.InefficientStringConcatenation(); // Causes performance issues
		dataHandler.ExceptionSwallowing(); // Swallows exceptions

		UserManager userManager = new UserManager();
		var user = userManager.ManageUsers();

		Logger logger = new Logger();
		logger.LogMessage( "Processed Data: " + processedData + " User: " + user );
	}
}