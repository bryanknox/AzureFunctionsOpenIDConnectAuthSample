namespace OidcApiAuthorization.Abstractions
{
    /// <summary>
    /// Encapsulates the results of an API authorization.
    /// </summary>
    public class ApiAuthorizationResult
    {
        /// <summary>
        /// Constructs a success authorization.
        /// </summary>
        public ApiAuthorizationResult()
        {
        }

        /// <summary>
        /// Constructs a failed authorization with given reason.
        /// </summary>
        /// <param name="failureReason">
        /// Describes the reason for the authorization failure.
        /// </param>
        public ApiAuthorizationResult(string failureReason)
        {
            FailureReason = failureReason;
        }

        /// <summary>
        /// True if authorization failed.
        /// </summary>
        public bool Failed => FailureReason != null;

        /// <summary>
        /// String describing the reason for the authorization failure.
        /// </summary>
        public string FailureReason { get; }

        /// <summary>
        /// True if authorization was successful.
        /// </summary>
        public bool Success => !Failed;
    }
}
