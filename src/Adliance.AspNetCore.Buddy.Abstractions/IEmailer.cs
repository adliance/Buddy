using System;
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
    /// <param name="email">The email to be sent.</param>
    /// <returns>A task.</returns>
    Task Send(SendableEmail email);

    /// <summary>
    /// Sends the specified message without blocking. The <paramref name="onCompleted"/> callback is invoked when sending finishes;
    /// a <see langword="null"/> exception value indicates success.
    /// </summary>
    void SendNonBlocking(Action<Exception?> onCompleted, SendableEmail email);
    
    Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);
    Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);
    Task Send(IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
    Task Send(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
    Task Send(IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
    Task Send(IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
    void SendNonBlocking(Action<Exception?> onCompleted, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);
    void SendNonBlocking(Action<Exception?> onCompleted, string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);
    void SendNonBlocking(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
    void SendNonBlocking(Action<Exception?> onCompleted, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
    void SendNonBlocking(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
    void SendNonBlocking(Action<Exception?> onCompleted, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
}
