using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

/// <summary>
/// Specifies the contract for a SMS messaging gateway.
/// </summary>
public interface ISmser
{
    /// <summary>
    /// Sends the provided <paramref name="text"/> to a <paramref name="recipient"/>. 
    /// </summary>
    /// <param name="recipient">The recipient of the message.</param>
    /// <param name="text">The text of the message you want to send.</param>
    /// <returns>A task.</returns>
    Task SendAsync(string recipient, string text);
}
