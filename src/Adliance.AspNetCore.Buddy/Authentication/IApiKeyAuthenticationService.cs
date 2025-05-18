using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Authentication;

/// <example>
/// services.AddAuthentication(options =>
/// {
///     // the scheme name has to match the value we're going to use in AuthenticationBuilder.AddScheme(...)
///     options.DefaultAuthenticateScheme = "api";
///     options.DefaultChallengeScheme = "api";
/// }).AddApiKey&lt;ApiKeyAuthenticationService&gt;("api", null);
/// </example>
public interface IApiKeyAuthenticationService
{
    /// <summary>
    /// Validates the in apiKey provided API key.
    /// </summary>
    /// <param name="apiKey">The API key to check as string.</param>
    /// <returns>A value if the API key is valid, and values indicating the username, roles and claims if the API key is valid.</returns>
    Task<ApiKeyAuthenticationServiceResult> ValidateKey(string? apiKey);
}

public class ApiKeyAuthenticationServiceResult
{
    public bool IsValid { get; set; }
    public string? UserName { get; set; }
    public IEnumerable<string>? Roles { get; set; }
    public IEnumerable<Claim>? Claims { get; set; }
}
