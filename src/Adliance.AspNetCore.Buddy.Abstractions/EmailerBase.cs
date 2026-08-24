using System;
using System.Linq;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

public abstract class EmailerBase(IEmailConfiguration emailConfig) : IEmailer
{
    public async Task Send(SendableEmail email)
    {
        if (emailConfig.Disable) return;

        var sender = email.Sender ?? new EmailSender
        {
            Name = emailConfig.SenderName,
            EmailAddress = emailConfig.SenderAddress,
            ReplyToEmailAddress = emailConfig.ReplyToAddress,
            ReplyToName = emailConfig.SenderName
        };
        var to = email.To.ToArray();
        var cc = email.Cc.ToArray();
        var bcc = email.Bcc.ToArray();
        var subject = email.Subject;
        var htmlBody = email.HtmlBody;
        var textBody = email.TextBody;
        var attachments = email.Attachments.ToArray();

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

    public void Send(Action<Exception?> onCompleted, SendableEmail email)
    {
        _ = RunInternal(() => Send(email), onCompleted);
    }

    private static async Task RunInternal(Func<Task> send, Action<Exception?> onCompleted)
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

    public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail("", recipientAddress, subject, htmlBody, textBody, attachments));

    public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody,
        params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(senderName, senderAddress, replyTo, recipientName, recipientAddress, subject, htmlBody, textBody, attachments));

    public async Task Send(IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(sender, to, [], [], subject, htmlBody, textBody, attachments));

    public async Task Send(IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(to, [], [], subject, htmlBody, textBody, attachments));

    public async Task Send(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(to, cc, bcc, subject, htmlBody, textBody, attachments));

    public async Task Send(IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody,
        params IEmailAttachment[] attachments)
        => await Send(new SendableEmail(sender, to, cc, bcc, subject, htmlBody, textBody, attachments));

    public void Send(Action<Exception?> onCompleted, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        => Send(onCompleted, new SendableEmail(recipientAddress, subject, htmlBody, textBody, attachments));

    public void Send(Action<Exception?> onCompleted, string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody,
        string textBody, params IEmailAttachment[] attachments)
        => Send(onCompleted, new SendableEmail(senderName, senderAddress, replyTo, recipientName, recipientAddress, subject, htmlBody, textBody, attachments));

    public void Send(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => Send(onCompleted, new SendableEmail(sender, to, subject, htmlBody, textBody, attachments));

    public void Send(Action<Exception?> onCompleted, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        => Send(onCompleted, new SendableEmail(to, subject, htmlBody, textBody, attachments));

    public void Send(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody,
        string? textBody, params IEmailAttachment[] attachments)
        => Send(onCompleted, new SendableEmail(sender, to, cc, bcc, subject, htmlBody, textBody, attachments));

    public void Send(Action<Exception?> onCompleted, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody,
        params IEmailAttachment[] attachments)
        => Send(onCompleted, new SendableEmail(to, cc, bcc, subject, htmlBody, textBody, attachments));
}
