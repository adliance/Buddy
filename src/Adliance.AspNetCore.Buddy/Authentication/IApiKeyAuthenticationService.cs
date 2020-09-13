using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Authentication
{
    /// <example>
    /// services.AddAuthentication(options =>
    /// {
    ///     // the scheme name has to match the value we're going to use in AuthenticationBuilder.AddScheme(...)
    ///     options.DefaultAuthenticateScheme = "api";
    ///     options.DefaultChallengeScheme = "api";
    /// })
    ///     .AddApiKey<ApiKeyAuthenticationService>("api", null);
    /// </example>
    public interface IApiKeyAuthenticationService
    {
        /// <summary>
        /// Validates the in <paramref name="apiKey"/> provided API key.
        /// </summary>
        /// <param name="apiKey">The API key to check as string.</param>
        /// <returns><value>True</value> if the <paramref name="apiKey"/> is valid else <value>false</value>.</returns>
        Task<bool> IsValidApiKey(string? apiKey);
        
        /// <summary>
        /// Validates the in <paramref name="apiKey"/> provided API key, and returns the user name (identifier) for the matching user.
        /// If no matching user is found, then null is returned.
        /// </summary>
        /// <param name="apiKey">The API key to check as string.</param>
        /// <returns>The user name, or null.</returns>
        Task<string> GetUserName(string? apiKey);
        
        /// <summary>
        /// Validates the in <paramref name="apiKey"/> provided API key, and returns the roles of the user.
        /// </summary>
        /// <param name="apiKey">The API key to check as string.</param>
        /// <returns>The roles of the user, or null if the user is not valid.</returns>
        Task<IEnumerable<string>> GetRoles(string? apiKey);
    }
}
