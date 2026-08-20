using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// The contract for an email renderer.
/// </summary>
public interface IEmailRenderer
{
    /// <summary>
    /// Renders several razor templates provided in the <paramref name="templateDirectoryName"/> directory.
    /// </summary>
    /// <param name="email">The email to be rendered.</param>
    /// <returns>A rendered email which can be sent using IEmailer.Send</returns>
    Task<SendableEmail> Render(RenderableEmail email);

    /// <summary>
    /// Renders several razor templates provided in the <paramref name="templateDirectoryName"/> directory and sends the result as an email.
    /// </summary>
    /// <param name="email">The email to be rendered and sent.</param>
    Task RenderAndSend(RenderableEmail email);

    /// <summary>
    /// Renders several razor templates provided in the <paramref name="templateDirectoryName"/> directory and sends the result as an email in a separate thread to avoid blocking.
    /// </summary>
    /// <param name="email">The email to be rendered and sent.</param>
    Task RenderAndSendNonBlocking(RenderableEmail email);
}
