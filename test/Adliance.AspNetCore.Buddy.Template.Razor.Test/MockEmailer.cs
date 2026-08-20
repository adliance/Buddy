using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

public class MockEmailer : IEmailer
{
    public List<SendableEmail> SentEmails { get; } = [];

    /// <summary>
    /// When set, Send()/SendNonBlocking() will fail with this exception instead of succeeding.
    /// </summary>
    public Exception? ExceptionToThrow { get; set; }

    public Task Send(SendableEmail email)
    {
        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }

        SentEmails.Add(email);
        return Task.CompletedTask;
    }

    public void SendNonBlocking(Action<Exception?> onCompleted, SendableEmail email)
    {
        if (ExceptionToThrow != null)
        {
            onCompleted(ExceptionToThrow);
            return;
        }

        SentEmails.Add(email);
        onCompleted(null);
    }

    // The obsolete IEmailer overloads below are not used by EmailRenderer and are therefore not needed by these tests.
    private static NotSupportedException Unused => new("This obsolete overload is not used by EmailRenderer.");

    public Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments) => throw Unused;
    public Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments) => throw Unused;
    public Task Send(IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments) => throw Unused;
    public Task Send(IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments) => throw Unused;
    public Task Send(IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments) => throw Unused;
    public Task Send(IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments) => throw Unused;
    public void SendNonBlocking(Action<Exception?> onCompleted, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments) => throw Unused;
    public void SendNonBlocking(Action<Exception?> onCompleted, string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments) => throw Unused;
    public void SendNonBlocking(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments) => throw Unused;
    public void SendNonBlocking(Action<Exception?> onCompleted, IEmailRecipient[] to, IEmailRecipient[] cc, IEmailRecipient[] bcc, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments) => throw Unused;
    public void SendNonBlocking(Action<Exception?> onCompleted, IEmailSender sender, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments) => throw Unused;
    public void SendNonBlocking(Action<Exception?> onCompleted, IEmailRecipient[] to, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments) => throw Unused;
}
