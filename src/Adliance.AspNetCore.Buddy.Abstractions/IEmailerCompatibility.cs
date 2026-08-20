using System;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

public partial interface IEmailer
{
    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    Task Send(IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    Task Send(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    Task Send(IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    Task Send(IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    void SendNonBlocking(Action<Exception?> onCompleted, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    void SendNonBlocking(Action<Exception?> onCompleted, string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    void SendNonBlocking(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    void SendNonBlocking(Action<Exception?> onCompleted, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    void SendNonBlocking(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    void SendNonBlocking(Action<Exception?> onCompleted, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments);
}
