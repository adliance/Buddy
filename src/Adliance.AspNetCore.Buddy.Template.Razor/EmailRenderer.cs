using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// Renders a razor template and sends the result as email.
/// </summary>
public class EmailRenderer : IEmailRenderer
{
    private readonly ITemplater _templater;
    private readonly IEmailer _mailer;

    public EmailRenderer(ITemplater templater, IEmailer mailer)
    {
        _templater = templater;
        _mailer = mailer;
    }

    /// <inheritdoc />
    /// <summary>
    /// Renders an e-mail, based on three templates.
    /// The specific template names will be generated from the specified <paramref name="templateBaseName"/>:
    /// "EmailTemplates/{templateBaseName}.Subject", "EmailTemplates/{templateBaseName}.Html", "EmailTemplates/{templateBaseName}.Text"
    /// </summary>
    public virtual async Task RenderAndSend(string recipientAddress, string templateBaseName, object viewModel,
        params IEmailAttachment[] attachments)
    {
        await RenderAndSend(
            recipientAddress,
            "EmailTemplates",
            $"{templateBaseName}.Subject",
            $"{templateBaseName}.Html",
            $"{templateBaseName}.Text",
            viewModel,
            attachments
        );
    }

    /// <inheritdoc />
    public virtual async Task RenderAndSend(string recipientAddress, string templateDirectoryName,
        string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel,
        params IEmailAttachment[] attachments)
    {
        var subject = (await _templater.Render(templateDirectoryName, $"{subjectTemplateName}", viewModel)).Trim();
        var html = (await _templater.Render(templateDirectoryName, $"{htmlTemplateName}", viewModel)).Trim();

        string text;
        try
        {
            text = (await _templater.Render(templateDirectoryName, $"{textTemplateName}", viewModel)).Trim();
        }
        catch // support e-mails without a text version, just to avoid having to "duplicate" the HTML template all the time
        {
            text = "";
        }

        await _mailer.Send(recipientAddress, subject, html, text, attachments);
    }
}
