using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Authentication;

/// <example>
/// services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme).AddBasicAuthenticationBasicAuthenticationService();
/// </example>
public interface IBasicAuthenticationService
{
    /// <summary>
    /// Authenticates a user by the specified <paramref name="username"/> and <paramref name="password"/> and
    /// returns the user if successful.
    /// </summary>
    /// <param name="username">The username of the user to check as string.</param>
    /// <param name="password">The password of the user to check as string.</param>
    /// <returns>The user on success or null if failed.</returns>
    Task<BasicAuthenticationUser> AuthenticateAsync(string username, string password);
}
