using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// The contract for an email renderer.
/// </summary>
public interface IEmailRenderer
{
    /// <summary>
    /// Renders a several razor templates provided in the "EmailTemplates" directory and sends the result as an email message.
    /// </summary>
    /// <param name="recipientAddress">The email address of the recipient of the message.</param>
    /// <param name="templateBaseName">The template's base name.</param>
    /// <param name="viewModel">The view model data for the razor template.</param>
    /// <param name="attachments">A collection of file attachments that will be included in the message.</param>
    Task RenderAndSend(string recipientAddress, string templateBaseName, object viewModel,
        params IEmailAttachment[] attachments);

    /// <summary>
    /// Renders several razor templates provided in the <paramref name="templateDirectoryName"/> directory
    /// and sends the result as an email message.
    /// </summary>
    /// <param name="recipientAddress">The email address of the recipient of the message.</param>
    /// <param name="templateDirectoryName">The directory the templates are located in.</param>
    /// <param name="subjectTemplateName">The template's name for the subject of the message.</param>
    /// <param name="htmlTemplateName">The template's name for the html formatted version of the message body.</param>
    /// <param name="textTemplateName">The template's name for the plain-text formatted version of the message body.</param>
    /// <param name="viewModel">The view model data for the razor template.</param>
    /// <param name="attachments">A collection of file attachments that will be included in the message.</param>
    Task RenderAndSend(string recipientAddress, string templateDirectoryName, string subjectTemplateName,
        string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments);
}
