using Microsoft.Extensions.Logging;

namespace TestFixtures.AzureFunctions
{ 
    /// <summary>
    /// A log entry for use in an ILogger related test fixture.
    /// </summary>
    public class LogEntry
    {
        public LogLevel LogLevel { get; set; }

        public string Message { get; set; }
    }
}
