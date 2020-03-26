namespace OidcApiAuthorization.Abstractions
{
    public interface IOidcApiAuthorizationSettings
    {
        string AuthorizationAudience { get; }

        string AuthorizationIssuerUrl { get; }
    }
}
