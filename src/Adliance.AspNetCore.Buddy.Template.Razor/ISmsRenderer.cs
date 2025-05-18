using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// The contract for a SMS renderer.
/// </summary>
public interface ISmsRenderer
{
    /// <summary>
    /// Renders a razor template provided in the "SmsTemplates" directory and sends the result as a SMS.
    /// </summary>
    /// <param name="recipientAddress">The recipient of the message.</param>
    /// <param name="templateBaseName">The template's base name.</param>
    /// <param name="viewModel">The view model data for the razor template.</param>
    Task RenderAndSendAsync(string recipientAddress, string templateBaseName, object viewModel);

    /// <summary>
    /// Renders a razor template with name <paramref name="textTemplateName"/> provided in the
    /// <paramref name="templateDirectoryName"/> directory and sends the result as a SMS.
    /// </summary>
    /// <param name="recipientAddress">The recipient of the message.</param>
    /// <param name="templateDirectoryName">The directory the template is located in.</param>
    /// <param name="textTemplateName">The template's base name.</param>
    /// <param name="viewModel">The view model data for the razor template.</param>
    Task RenderAndSendAsync(string recipientAddress, string templateDirectoryName, string textTemplateName, object viewModel);
}
