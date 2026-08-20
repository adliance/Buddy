using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

public sealed class RenderableEmail(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc,
    string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel,
    params IEmailAttachment[] attachments)
{
    /// <summary>
    /// The addresses in the To header are the primary recipients of the message.
    /// </summary>
    public IEmailRecipient[] To { get; } = to;
    /// <summary>
    /// The addresses in the CC header are the secondary recipients of the message, which should receive a copy.
    /// </summary>
    public IEmailRecipient[] Cc { get; } = cc;
    /// <summary>
    /// The addresses in the BCC header are secondary recipients of the message, which should receive a copy without information about other recipients.
    /// </summary>
    public IEmailRecipient[] Bcc { get; } = bcc;
    /// <summary>
    /// Represents a collection of file attachments that will be included in the message.
    /// </summary>
    public IEmailAttachment[] Attachments { get; } = attachments;

    /// <summary>
    /// The directory the templates are located in.
    /// </summary>
    public string TemplateDirectoryName { get; } = templateDirectoryName;
    /// <summary>
    /// The template's name for the subject of the message.
    /// </summary>
    public string SubjectTemplateName { get; } = subjectTemplateName;
    /// <summary>
    /// The template's name for the html formatted version of the message body.
    /// </summary>
    public string HtmlTemplateName { get; } = htmlTemplateName;
    /// <summary>
    /// The template's name for the plain-text formatted version of the message body.
    /// </summary>
    public string TextTemplateName { get; } = textTemplateName;
    /// <summary>
    /// The view model data for the razor template.
    /// </summary>
    public object ViewModel { get; } = viewModel;

    public RenderableEmail(IEmailRecipient[] to, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments) : this(
        to,
        [],
        [],
        templateDirectoryName,
        subjectTemplateName,
        htmlTemplateName,
        textTemplateName,
        viewModel,
        attachments) { }

    public RenderableEmail(string recipientAddress, string templateBaseName, object viewModel, params IEmailAttachment[] attachments) : this(
        recipientAddress,
        DefaultTemplateDirectoryName,
        ToSubjectTemplateName(templateBaseName),
        ToHtmlTemplateName(templateBaseName),
        ToTextTemplateName(templateBaseName),
        viewModel,
        attachments) { }

    public RenderableEmail(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string templateBaseName, object viewModel, params IEmailAttachment[] attachments) : this(
        to,
        cc,
        bcc,
        DefaultTemplateDirectoryName,
        ToSubjectTemplateName(templateBaseName),
        ToHtmlTemplateName(templateBaseName),
        ToTextTemplateName(templateBaseName),
        viewModel,
        attachments) { }

    public RenderableEmail(IEmailRecipient[] to, string templateBaseName, object viewModel, params IEmailAttachment[] attachments) : this(
        to,
        [],
        [],
        DefaultTemplateDirectoryName,
        ToSubjectTemplateName(templateBaseName),
        ToHtmlTemplateName(templateBaseName),
        ToTextTemplateName(templateBaseName),
        viewModel,
        attachments) { }

    public RenderableEmail(string recipientAddress, string templateDirectoryName, string subjectTemplateName, string htmlTemplateName, string textTemplateName, object viewModel, params IEmailAttachment[] attachments) : this(
        [new EmailSenderRecipient
        {
            Name = recipientAddress,
            EmailAddress = recipientAddress,
        }],
        [],
        [],
        templateDirectoryName,
        subjectTemplateName,
        htmlTemplateName,
        textTemplateName,
        viewModel,
        attachments
        ) { }

    private const string DefaultTemplateDirectoryName = "EmailTemplates";
    private static string ToSubjectTemplateName(string baseName) => $"{baseName}.Subject";
    private static string ToHtmlTemplateName(string baseName) => $"{baseName}.Html";
    private static string ToTextTemplateName(string baseName) => $"{baseName}.Text";
}
