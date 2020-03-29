namespace OidcApiAuthorization.Models
{
    public class HealthCheckResult
    {
        /// <summary>
        /// Construt a HealthCheckResult that indicates good health.
        /// </summary>
        public HealthCheckResult()
        {
        }

        /// <summary>
        /// Construt a HealthCheckResult that indicates bad health.
        /// </summary>
        /// <param name="badHealthMessage">
        /// The message describing the bad health.
        /// </param>
        public HealthCheckResult(string badHealthMessage)
        {
            BadHealthMessage = badHealthMessage;
        }

        public bool IsHealthy => BadHealthMessage == null;

        public string BadHealthMessage { get; set; }
    }
}
