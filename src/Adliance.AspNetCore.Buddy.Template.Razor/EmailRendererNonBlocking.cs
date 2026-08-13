using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// Renders a razor template and sends the result as an email without blocking.
/// </summary>
public class EmailRendererNonBlocking(ITemplater templater, IEmailer mailer, ILogger<EmailRendererNonBlocking> logger) : IEmailRendererNonBlocking
{
    private const string DefaultTemplateDirectoryName = "EmailTemplates";
    private static string ToSubjectTemplateName(string baseName) => $"{baseName}.Subject";
    private static string ToHtmlTemplateName(string baseName) => $"{baseName}.Html";
    private static string ToTextTemplateName(string baseName) => $"{baseName}.Text";

    /// <inheritdoc />
    /// <summary>
    /// Renders an e-mail, based on three templates without blocking.
    /// The specific template names will be generated from the specified <paramref name="templateBaseName"/>:
    /// "EmailTemplates/{templateBaseName}.Subject", "EmailTemplates/{templateBaseName}.Html", "EmailTemplates/{templateBaseName}.Text"
    /// </summary>
    public virtual async Task RenderAndSendNonBlocking(string recipientAddress, string templateBaseName, object viewModel,
        params IEmailAttachment[] attachments)
    {
        await RenderAndSendNonBlocking(
            recipientAddress,
            DefaultTemplateDirectoryName,
            ToSubjectTemplateName(templateBaseName),
            ToHtmlTemplateName(templateBaseName),
            ToTextTemplateName(templateBaseName),
            viewModel,
            attachments
        );
    }

    public virtual async Task RenderAndSendNonBlocking(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateBaseName,
        object viewModel, params IEmailAttachment[] attachments)
    {
        await RenderAndSendNonBlocking(
            to,
            cc,
            bcc,
            DefaultTemplateDirectoryName,
            ToSubjectTemplateName(templateBaseName),
            ToHtmlTemplateName(templateBaseName),
            ToTextTemplateName(templateBaseName),
            viewModel,
            attachments
        );
    }

    public Task RenderAndSendNonBlockingAsync(IEmailRecipient[] to, string templateBaseName, object viewModel, params IEmailAttachment[] attachments)
    {
        throw new System.NotImplementedException();
    }

    public virtual async Task RenderAndSendNonBlocking(IEmailRecipient[] to, string templateBaseName, object viewModel,
        params IEmailAttachment[] attachments)
    {
        await RenderAndSendNonBlocking(
            to,
            [],
            [],
            DefaultTemplateDirectoryName,
            ToSubjectTemplateName(templateBaseName),
            ToHtmlTemplateName(templateBaseName),
            ToTextTemplateName(templateBaseName),
            viewModel,
            attachments
        );
    }

    public virtual async Task RenderAndSendNonBlocking(string recipientAddress, string templateDirectoryName,
        string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel,
        params IEmailAttachment[] attachments)
    {
        var recipient = new EmailSenderRecipient
        {
            Name = recipientAddress,
            EmailAddress = recipientAddress,
        };

        await RenderAndSendNonBlocking(
            [recipient],
            [],
            [],
            templateDirectoryName,
            subjectTemplateName,
            htmlTemplateName,
            textTemplateName,
            viewModel,
            attachments
        );
    }

    public virtual async Task RenderAndSendNonBlocking(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateDirectoryName,
        string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel,
        params IEmailAttachment[] attachments)
    {
        var subject = (await templater.Render(templateDirectoryName, $"{subjectTemplateName}", viewModel)).Trim();
        var html = (await templater.Render(templateDirectoryName, $"{htmlTemplateName}", viewModel)).Trim();

        string text;
        try
        {
            text = (await templater.Render(templateDirectoryName, $"{textTemplateName}", viewModel)).Trim();
        }
        catch // support e-mails without a text version, just to avoid having to "duplicate" the HTML template all the time
        {
            text = "";
        }

        mailer.SendNonBlocking(ex =>

        {
            if (ex != null)
            {
                logger.LogError(ex, "Sending mail failed.");
            }
        }, to, cc, bcc, subject, html, text, attachments);
    }

    public virtual async Task RenderAndSendNonBlocking(IEmailRecipient[] to, string templateDirectoryName, string subjectTemplateName,
        string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments)
    {
        await RenderAndSendNonBlocking(
            to,
            [],
            [],
            templateDirectoryName,
            subjectTemplateName,
            htmlTemplateName,
            textTemplateName,
            viewModel,
            attachments
        );
    }

    private sealed class EmailSenderRecipient : IEmailSender, IEmailRecipient
    {
        public required string Name { get; init; }
        public required string EmailAddress { get; set; }
        public string? ReplyToEmailAddress { get; init; }
        public string? ReplyToName { get; init; }
    }
}
