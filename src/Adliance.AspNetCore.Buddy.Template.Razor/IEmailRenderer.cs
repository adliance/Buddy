using System;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// The contract for an email renderer.
/// </summary>
public interface IEmailRenderer
{
    /// <summary>
    /// Renders several razor templates provided in the templateDirectoryName directory.
    /// </summary>
    /// <param name="email">The email to be rendered.</param>
    /// <returns>A rendered email which can be sent using IEmailer.Send</returns>
    Task<SendableEmail> Render(RenderableEmail email);

    /// <summary>
    /// Renders several razor templates provided in the templateDirectoryName directory and sends the result as an email.
    /// </summary>
    /// <param name="onCompleted">Callback that fires when e-mail sending has been completed. A null exception indicates success.</param>
    /// <param name="email">The email to be rendered and sent.</param>
    Task RenderAndSend(Action<Exception?> onCompleted, RenderableEmail email);

    /// <summary>
    /// Renders several razor templates provided in the templateDirectoryName directory and sends the result as an email.
    /// </summary>
    /// <param name="email">The email to be rendered and sent.</param>
    Task RenderAndSend(RenderableEmail email);
    Task RenderAndSend(string recipientAddress, string templateBaseName, object viewModel, params IEmailAttachment[] attachments);
    Task RenderAndSend(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateBaseName, object viewModel, params IEmailAttachment[] attachments);
    Task RenderAndSend(IEmailRecipient[] to, string templateBaseName, object viewModel, params IEmailAttachment[] attachments);
    Task RenderAndSend(string recipientAddress, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments);
    Task RenderAndSend(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments);
    Task RenderAndSend(IEmailRecipient[] to, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments);

}
