using System;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

public abstract class EmailerBase(IEmailConfiguration emailConfig) : IEmailer
{
    public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
    {
        await Send(emailConfig.SenderName, emailConfig.SenderAddress, emailConfig.ReplyToAddress, "", recipientAddress, subject, htmlBody, textBody, attachments);
    }

    public async Task Send(
        string senderName,
        string senderAddress,
        string replyTo,
        string recipientName,
        string recipientAddress,
        string subject,
        string htmlBody,
        string textBody,
        params IEmailAttachment[] attachments)
    {
        var sender = new EmailSenderRecipient
        {
            Name = senderName,
            EmailAddress = senderAddress,
            ReplyToEmailAddress = replyTo,
            ReplyToName = senderName
        };

        var recipient = new EmailSenderRecipient
        {
            Name = recipientName,
            EmailAddress = recipientAddress
        };

        await Send(sender, [recipient], subject, htmlBody, textBody, attachments);
    }

    public async Task Send(IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
    {
        await Send(sender, to, [], [], subject, htmlBody, textBody, attachments);
    }

    public async Task Send(IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
    {
        await Send(to, [], [], subject, htmlBody, textBody, attachments);
    }

    public async Task Send(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
    {
        var sender = new EmailSenderRecipient
        {
            Name = emailConfig.SenderName,
            EmailAddress = emailConfig.SenderAddress,
            ReplyToEmailAddress = emailConfig.ReplyToAddress,
            ReplyToName = emailConfig.SenderName
        };
        await Send(sender, to, cc, bcc, subject, htmlBody, textBody, attachments);
    }

    public async Task Send(
        IEmailSender sender,
        IEmailRecipient[] to,
        IEmailRecipient[] cc,
        IEmailRecipient[] bcc,
        string subject,
        string htmlBody,
        string? textBody,
        params IEmailAttachment[] attachments)
    {
        if (emailConfig.Disable) return;

        if (to.Length == 0) throw new Exception("Not a single recipient specified.");

        if (!string.IsNullOrEmpty(emailConfig.RedirectAllEmailsTo))
        {
            foreach (var x in to) x.EmailAddress = emailConfig.RedirectAllEmailsTo;
            foreach (var x in cc) x.EmailAddress = emailConfig.RedirectAllEmailsTo;
            foreach (var x in bcc) x.EmailAddress = emailConfig.RedirectAllEmailsTo;
        }

        if (!string.IsNullOrEmpty(emailConfig.SubjectPrefix)) subject = emailConfig.SubjectPrefix + subject;
        if (!string.IsNullOrEmpty(emailConfig.SubjectPostfix)) subject += emailConfig.SubjectPostfix;

        await SendInternal(sender, to, cc, bcc, subject, htmlBody, textBody, attachments);
    }

    public void SendNonBlocking(
        Action<Exception?> onCompleted,
        string recipientAddress,
        string subject,
        string htmlBody,
        string textBody,
        params IEmailAttachment[] attachments)
    {
        _ = RunNonBlocking(() => Send(recipientAddress, subject, htmlBody, textBody, attachments), onCompleted);
    }

    public void SendNonBlocking(
        Action<Exception?> onCompleted,
        string senderName,
        string senderAddress,
        string replyTo,
        string recipientName,
        string recipientAddress,
        string subject,
        string htmlBody,
        string textBody,
        params IEmailAttachment[] attachments)
    {
        _ = RunNonBlocking(() => Send(senderName, senderAddress, replyTo, recipientName, recipientAddress, subject, htmlBody, textBody, attachments), onCompleted);
    }

    public void SendNonBlocking(
        Action<Exception?> onCompleted,
        IEmailSender sender,
        IEmailRecipient[] to,
        string subject,
        string htmlBody,
        string? textBody,
        params IEmailAttachment[] attachments)
    {
        _ = RunNonBlocking(() => Send(sender, to, subject, htmlBody, textBody, attachments), onCompleted);
    }

    public void SendNonBlocking(
        Action<Exception?> onCompleted,
        IEmailRecipient[] to,
        string subject,
        string htmlBody,
        string? textBody,
        params IEmailAttachment[] attachments)
    {
        _ = RunNonBlocking(() => Send(to, subject, htmlBody, textBody, attachments), onCompleted);
    }

    public void SendNonBlocking(
        Action<Exception?> onCompleted,
        IEmailSender sender,
        IEmailRecipient[] to,
        IEmailRecipient[] cc,
        IEmailRecipient[] bcc,
        string subject,
        string htmlBody,
        string? textBody,
        params IEmailAttachment[] attachments)
    {
        _ = RunNonBlocking(() => Send(sender, to, cc, bcc, subject, htmlBody, textBody, attachments), onCompleted);
    }

    public void SendNonBlocking(
        Action<Exception?> onCompleted,
        IEmailRecipient[] to,
        IEmailRecipient[] cc,
        IEmailRecipient[] bcc,
        string subject,
        string htmlBody,
        string? textBody,
        params IEmailAttachment[] attachments)
    {
        _ = RunNonBlocking(() => Send(to, cc, bcc, subject, htmlBody, textBody, attachments), onCompleted);
    }

    private static async Task RunNonBlocking(Func<Task> send, Action<Exception?> onCompleted)
    {
        try
        {
            await Task.Run(send).ConfigureAwait(false);
            onCompleted(null);
        }
        catch (Exception ex)
        {
            onCompleted(ex);
        }
    }

    protected abstract Task SendInternal(
        IEmailSender sender,
        IEmailRecipient[] to,
        IEmailRecipient[] cc,
        IEmailRecipient[] bcc,
        string subject,
        string htmlBody,
        string? textBody,
        params IEmailAttachment[] attachments);

    private sealed class EmailSenderRecipient : IEmailSender, IEmailRecipient
    {
        public required string Name { get; init; }
        public required string EmailAddress { get; set; }
        public string? ReplyToEmailAddress { get; init; }
        public string? ReplyToName { get; init; }
    }
}
