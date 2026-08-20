using System;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// The contract for an email renderer.
/// </summary>
public partial interface IEmailRenderer
{
    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    Task RenderAndSend(string recipientAddress, string templateBaseName, object viewModel, params IEmailAttachment[] attachments);

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]

    Task RenderAndSend(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateBaseName, object viewModel, params IEmailAttachment[] attachments);

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    Task RenderAndSend(IEmailRecipient[] to, string templateBaseName, object viewModel, params IEmailAttachment[] attachments);

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    Task RenderAndSend(string recipientAddress, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments);

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    Task RenderAndSend(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments);

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    Task RenderAndSend(IEmailRecipient[] to, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments);
}
