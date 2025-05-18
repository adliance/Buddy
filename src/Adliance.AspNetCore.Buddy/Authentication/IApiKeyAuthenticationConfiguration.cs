namespace Adliance.AspNetCore.Buddy.Authentication;

/// <summary>
/// The configuration for the <see cref="ApiKeyAuthenticationHandler"/>.
/// </summary>
public interface IApiKeyAuthenticationConfiguration
{
    /// <summary>
    /// The name of the HTTP header to check for API key, eg. "X-API-Key".
    /// </summary>
    string HttpHeaderKey { get; }

    /// <summary>
    /// The name of the query string to check for the API key, eg. "key".
    /// </summary>
    string QueryStringKey { get; }

    /// <summary>
    /// Allows to accept an empty or no specified API key.
    /// </summary>
    bool AllowEmptyApiKey { get; }
}
