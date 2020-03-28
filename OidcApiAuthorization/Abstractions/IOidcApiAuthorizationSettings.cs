namespace OidcApiAuthorization.Abstractions
{
    public interface IOidcApiAuthorizationSettings
    {
        string Audience { get; }

        string IssuerUrl { get; }
    }
}
