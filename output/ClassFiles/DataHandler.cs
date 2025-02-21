using System;

namespace InputProcessor
{
    public class DataHandler
    {
        private readonly ILogger _logger;

        public DataHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int ComputeSum(int num1, int num2)
        {
            _logger.LogDebug("Computing sum of two integers.");
            return num1 + num2;
        }
        
        public void DisplayProcessedData(int result)
        {
            _logger.LogInformation($"Calculated Result: {result}");
        }
        
        public void SimulateIterativeProcess(int iterations)
        {
            if (iterations < 0)
                throw new ArgumentOutOfRangeException(nameof(iterations), "Iterations cannot be negative.");

            _logger.LogDebug("Starting iterative simulation process.");
            for (int i = 0; i < iterations; i++)
            {
                if (i % 1000 == 0) // Reducing the frequency of logging to every 1000 iterations
                {
                    _logger.LogDebug($"Iteration: {i}");
                }
            }
        }
    }
}