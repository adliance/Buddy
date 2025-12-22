using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

/// <summary>
/// Specifies the contract for an email provider.
/// </summary>
public interface IEmailer
{
    /// <summary>
    /// Asynchronously send the specified message.
    /// </summary>
    /// <param name="recipientAddress">The addresses in the To header are the primary recipients of the message.</param>
    /// <param name="subject">The Subject is typically a short string denoting the topic of the message.</param>
    /// <param name="htmlBody">Represents the html formatted version of the message body.</param>
    /// <param name="textBody">Represents the plain-text formatted version of the message body.</param>
    /// <param name="attachments">Represents a collection of file attachments that will be included in the message.</param>
    /// <returns>A task.</returns>
    Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);

    /// <summary>
    /// Asynchronously send the specified message.
    /// </summary>
    /// <param name="senderName">The name of the sender.</param>
    /// <param name="senderAddress">The e-mail address of the sender.</param>
    /// <param name="replyTo">The e-mail address to which replies should be sent.</param>
    /// <param name="recipientName">The name of the recipient.</param>
    /// <param name="recipientAddress">The addresses in the To header are the primary recipients of the message.</param>
    /// <param name="subject">The Subject is typically a short string denoting the topic of the message.</param>
    /// <param name="htmlBody">Represents the html formatted version of the message body.</param>
    /// <param name="textBody">Represents the plain-text formatted version of the message body.</param>
    /// <param name="attachments">Represents a collection of file attachments that will be included in the message.</param>
    /// <returns>A task.</returns>
    Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody,
        params IEmailAttachment[] attachments);

    /// <summary>
    /// Asynchronously send the specified message.
    /// </summary>
    /// <param name="sender">Name and e-mail address to show as the message origin.</param>
    /// <param name="to">The addresses in the To header are the primary recipients of the message.</param>
    /// <param name="cc">The addresses in the CC header are the secondary recipients of the message, which should receive a copy.</param>
    /// <param name="bcc">The addresses in the BCC header are secondary recipients of the message, which should receive a copy without information about other recipients.</param>
    /// <param name="subject">The Subject is typically a short string denoting the topic of the message.</param>
    /// <param name="htmlBody">Represents the html formatted version of the message body.</param>
    /// <param name="textBody">Represents the plain-text formatted version of the message body.</param>
    /// <param name="attachments">Represents a collection of file attachments that will be included in the message.</param>
    /// <returns>A task.</returns>
    Task Send(IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    /// <summary>
    /// Asynchronously send the specified message. Sender information from the config is used.
    /// </summary>
    /// <param name="to">The addresses in the To header are the primary recipients of the message.</param>
    /// <param name="cc">The addresses in the CC header are the secondary recipients of the message, which should receive a copy.</param>
    /// <param name="bcc">The addresses in the BCC header are secondary recipients of the message, which should receive a copy without information about other recipients.</param>
    /// <param name="subject">The Subject is typically a short string denoting the topic of the message.</param>
    /// <param name="htmlBody">Represents the html formatted version of the message body.</param>
    /// <param name="textBody">Represents the plain-text formatted version of the message body.</param>
    /// <param name="attachments">Represents a collection of file attachments that will be included in the message.</param>
    /// <returns>A task.</returns>
    Task Send(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    /// <summary>
    /// Asynchronously send the specified message.
    /// </summary>
    /// <param name="sender">Name and e-mail address to show as the message origin.</param>
    /// <param name="to">The addresses in the To header are the primary recipients of the message.</param>
    /// <param name="subject">The Subject is typically a short string denoting the topic of the message.</param>
    /// <param name="htmlBody">Represents the html formatted version of the message body.</param>
    /// <param name="textBody">Represents the plain-text formatted version of the message body.</param>
    /// <param name="attachments">Represents a collection of file attachments that will be included in the message.</param>
    /// <returns>A task.</returns>
    Task Send(IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    /// <summary>
    /// Asynchronously send the specified message Sender information from the config is used.
    /// </summary>
    /// <param name="to">The addresses in the To header are the primary recipients of the message.</param>
    /// <param name="subject">The Subject is typically a short string denoting the topic of the message.</param>
    /// <param name="htmlBody">Represents the html formatted version of the message body.</param>
    /// <param name="textBody">Represents the plain-text formatted version of the message body.</param>
    /// <param name="attachments">Represents a collection of file attachments that will be included in the message.</param>
    /// <returns>A task.</returns>
    Task Send(IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
}
