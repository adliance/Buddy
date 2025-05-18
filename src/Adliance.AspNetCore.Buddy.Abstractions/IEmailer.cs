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

    Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);
}
