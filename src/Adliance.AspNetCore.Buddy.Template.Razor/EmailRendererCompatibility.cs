using System;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

public partial class EmailRenderer : IEmailRenderer
{
    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    public virtual async Task RenderAndSend(string recipientAddress, string templateBaseName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(recipientAddress, templateBaseName, viewModel, attachments));

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    public virtual async Task RenderAndSend(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateBaseName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(to, cc, bcc, templateBaseName, viewModel, attachments));

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    public virtual async Task RenderAndSend(IEmailRecipient[] to, string templateBaseName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(to, templateBaseName, viewModel, attachments));

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    public virtual async Task RenderAndSend(string recipientAddress, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(recipientAddress, templateDirectoryName, subjectTemplateName, htmlTemplateName, textTemplateName, viewModel, attachments));

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    public virtual async Task RenderAndSend(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(to, cc, bcc, templateDirectoryName, subjectTemplateName, htmlTemplateName, textTemplateName, viewModel, attachments));

    [Obsolete("Use RenderAndSend() with a new instance of RenderableEmail instead")]
    public virtual async Task RenderAndSend(IEmailRecipient[] to, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(to, templateDirectoryName, subjectTemplateName, htmlTemplateName, textTemplateName, viewModel, attachments));
}
