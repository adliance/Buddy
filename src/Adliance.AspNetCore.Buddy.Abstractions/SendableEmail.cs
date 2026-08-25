namespace Adliance.AspNetCore.Buddy.Abstractions;

public sealed class SendableEmail(
    IEmailSender? sender,
    IEmailRecipient[] to,
    IEmailRecipient[] cc,
    IEmailRecipient[] bcc,
    string subject,
    string htmlBody,
    string? textBody,
    IEmailAttachment[] attachments)
{
    /// <summary>
    /// Name and e-mail address to show as the message origin.
    /// </summary>
    /// <remarks>
    /// Taken from config if null
    /// </remarks>
    public IEmailSender? Sender { get; } = sender;

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
    /// The Subject is typically a short string denoting the topic of the message.
    /// </summary>
    public string Subject { get; } = subject;

    /// <summary>
    /// Represents the html formatted version of the message body.
    /// </summary>
    public string HtmlBody { get; } = htmlBody;

    /// <summary>
    /// Represents the plain-text formatted version of the message body.
    /// </summary>
    public string? TextBody { get; } = textBody;

    /// <summary>
    /// Represents a collection of file attachments that will be included in the message.
    /// </summary>
    public IEmailAttachment[] Attachments { get; } = attachments;

    public SendableEmail(string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments) :
        this(sender: null,
            [
                new EmailRecipient
                {
                    Name = recipientName,
                    EmailAddress = recipientAddress
                }
            ],
            subject, htmlBody, textBody, attachments)
    {
    }

    public SendableEmail(
        string senderName,
        string senderAddress,
        string replyTo,
        string recipientName,
        string recipientAddress,
        string subject,
        string htmlBody,
        string textBody,
        params IEmailAttachment[] attachments) : this(
        new EmailSender
        {
            Name = senderName,
            EmailAddress = senderAddress,
            ReplyToEmailAddress = replyTo,
            ReplyToName = senderName
        },
        [
            new EmailRecipient
            {
                Name = recipientName,
                EmailAddress = recipientAddress
            }
        ],
        subject, htmlBody, textBody, attachments
    )
    {
    }

    public SendableEmail(IEmailSender? sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        : this(sender, to, [], [], subject, htmlBody, textBody, attachments)
    {
    }

    public SendableEmail(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        : this("", recipientAddress, subject, htmlBody, textBody, attachments)
    {
    }

    public SendableEmail(IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        : this(to, [], [], subject, htmlBody, textBody, attachments)
    {
    }

    public SendableEmail(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        : this(null, to, cc, bcc, subject, htmlBody, textBody, attachments)
    {
    }
}
