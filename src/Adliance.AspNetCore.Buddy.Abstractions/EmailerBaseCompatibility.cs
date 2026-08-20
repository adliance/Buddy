using System;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

public abstract partial class EmailerBase : IEmailer
{
    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail("", recipientAddress, subject, htmlBody, textBody, attachments));

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(senderName, senderAddress, replyTo, recipientName, recipientAddress, subject, htmlBody, textBody, attachments));

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    public async Task Send(IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(sender, to, [], [], subject, htmlBody, textBody, attachments));

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    public async Task Send(IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(to, [], [], subject, htmlBody, textBody, attachments));

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    public async Task Send(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(to, cc, bcc, subject, htmlBody, textBody, attachments));

    [Obsolete("Use Send() with a new instance of SendableEmail instead")]
    public async Task Send(IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(sender, to, cc, bcc, subject, htmlBody, textBody, attachments));

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    public void SendNonBlocking(Action<Exception?> onCompleted, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        => SendNonBlocking(onCompleted, new SendableEmail(recipientAddress, subject, htmlBody, textBody, attachments));

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    public void SendNonBlocking(Action<Exception?> onCompleted, string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        => SendNonBlocking(onCompleted, new SendableEmail(senderName, senderAddress, replyTo, recipientName, recipientAddress, subject, htmlBody, textBody, attachments));

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    public void SendNonBlocking(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => SendNonBlocking(onCompleted, new SendableEmail(sender, to, subject, htmlBody, textBody, attachments));

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    public void SendNonBlocking(Action<Exception?> onCompleted, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => SendNonBlocking(onCompleted, new SendableEmail(to, subject, htmlBody, textBody, attachments));

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    public void SendNonBlocking(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => SendNonBlocking(onCompleted, new SendableEmail(sender, to, cc, bcc, subject, htmlBody, textBody, attachments));

    [Obsolete("Use SendNonBlocking() with a new instance of SendableEmail instead")]
    public void SendNonBlocking(Action<Exception?> onCompleted, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => SendNonBlocking(onCompleted, new SendableEmail(to, cc, bcc, subject, htmlBody, textBody, attachments));
}
