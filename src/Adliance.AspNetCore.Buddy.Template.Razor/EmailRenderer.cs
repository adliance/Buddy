using System;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// Renders a razor template and sends the result as an email.
/// </summary>
public class EmailRenderer(ITemplater templater, IEmailer mailer) : IEmailRenderer
{
    /// <summary>
    /// Renders an e-mail, based on three templates.
    /// The specific template names will be generated from the specified templateBaseName:
    /// "EmailTemplates/{templateBaseName}.Subject", "EmailTemplates/{templateBaseName}.Html", "EmailTemplates/{templateBaseName}.Text"
    /// </summary>
    public virtual async Task<SendableEmail> Render(RenderableEmail email)
    {
        var subject = (await templater.Render(email.TemplateDirectoryName, email.SubjectTemplateName, email.ViewModel)).Trim();
        var html = (await templater.Render(email.TemplateDirectoryName, email.HtmlTemplateName, email.ViewModel)).Trim();

        string text;
        try
        {
            text = (await templater.Render(email.TemplateDirectoryName, $"{email.TextTemplateName}", email.ViewModel)).Trim();
        }
        catch // support e-mails without a text version, just to avoid having to "duplicate" the HTML template all the time
        {
            text = "";
        }

        return new SendableEmail(null, email.To, email.Cc, email.Bcc, subject, html, text, email.Attachments);
    }

    public virtual async Task RenderAndSend(RenderableEmail email)
    {
        await mailer.Send(await Render(email));
    }

    public virtual async Task RenderAndSend(Action<Exception?> onCompleted, RenderableEmail email)
    {
        var sendableEmail = await Render(email);
        mailer.Send(onCompleted, sendableEmail);
    }

    public virtual async Task RenderAndSend(string recipientAddress, string templateBaseName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(recipientAddress, templateBaseName, viewModel, attachments));

    public virtual async Task RenderAndSend(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateBaseName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(to, cc, bcc, templateBaseName, viewModel, attachments));

    public virtual async Task RenderAndSend(IEmailRecipient[] to, string templateBaseName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(to, templateBaseName, viewModel, attachments));

    public virtual async Task RenderAndSend(string recipientAddress, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel,
        params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(recipientAddress, templateDirectoryName, subjectTemplateName, htmlTemplateName, textTemplateName, viewModel, attachments));

    public virtual async Task RenderAndSend(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName,
        string textTemplateName, object viewModel, params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(to, cc, bcc, templateDirectoryName, subjectTemplateName, htmlTemplateName, textTemplateName, viewModel, attachments));

    public virtual async Task RenderAndSend(IEmailRecipient[] to, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel,
        params IEmailAttachment[] attachments)
        => await RenderAndSend(new RenderableEmail(to, templateDirectoryName, subjectTemplateName, htmlTemplateName, textTemplateName, viewModel, attachments));
}
