using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// Renders a razor template and sends the result as an email.
/// </summary>
public class EmailRenderer(ITemplater templater, IEmailer mailer, ILogger<EmailRenderer> logger) : IEmailRenderer
{
    /// <summary>
    /// Renders an e-mail, based on three templates.
    /// The specific template names will be generated from the specified templateBaseName:
    /// "EmailTemplates/{templateBaseName}.Subject", "EmailTemplates/{templateBaseName}.Html", "EmailTemplates/{templateBaseName}.Text"
    /// </summary>
    public virtual async Task<SendableEmail> Render(RenderableEmail email)
    {
        var subject = (await templater.Render(email.TemplateDirectoryName, $"{email.SubjectTemplateName}", email.ViewModel)).Trim();
        var html = (await templater.Render(email.TemplateDirectoryName, $"{email.HtmlTemplateName}", email.ViewModel)).Trim();

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

    public virtual async Task RenderAndSendNonBlocking(RenderableEmail email)
    {
        mailer.SendNonBlocking(ex =>
        {
            if (ex != null)
            {
                logger.LogError(ex, "Sending mail failed.");
            }
        }, await Render(email));
    }
}
