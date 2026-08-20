using System;
using System.Linq;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

public abstract class EmailerBase(IEmailConfiguration emailConfig) : IEmailer
{
    public async Task Send(SendableEmail email)
    {
        if (emailConfig.Disable)
            return;

        var sender = email.Sender ?? new EmailSenderRecipient
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

        if (to.Length == 0)
            throw new Exception("Not a single recipient specified.");

        if (!string.IsNullOrEmpty(emailConfig.RedirectAllEmailsTo))
        {
            foreach (var x in to) x.EmailAddress = emailConfig.RedirectAllEmailsTo;
            foreach (var x in cc) x.EmailAddress = emailConfig.RedirectAllEmailsTo;
            foreach (var x in bcc) x.EmailAddress = emailConfig.RedirectAllEmailsTo;
        }

        if (!string.IsNullOrEmpty(emailConfig.SubjectPrefix))
            subject = emailConfig.SubjectPrefix + subject;
        if (!string.IsNullOrEmpty(emailConfig.SubjectPostfix))
            subject += emailConfig.SubjectPostfix;

        await SendInternal(sender, to, cc, bcc, subject, htmlBody, textBody, attachments);
    }

    public void SendNonBlocking(Action<Exception?> onCompleted, SendableEmail email)
    {
        _ = RunNonBlocking(() => Send(email), onCompleted);
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
}
