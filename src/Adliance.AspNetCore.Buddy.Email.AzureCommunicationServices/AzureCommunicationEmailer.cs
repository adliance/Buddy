using Adliance.AspNetCore.Buddy.Abstractions;
using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.StaticFiles;

namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices;

public class AzureCommunicationEmailer(IAzureCommunicationConfiguration azureCommunicationConfig, IEmailConfiguration emailConfig) : IEmailer
{
    public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
    {
        await Send(emailConfig.SenderName, emailConfig.SenderAddress, emailConfig.ReplyToAddress, "", recipientAddress, subject, htmlBody, textBody, attachments);
    }

    public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
    {
        if (string.IsNullOrWhiteSpace(recipientAddress)) throw new ArgumentOutOfRangeException(nameof(recipientAddress));

        if (emailConfig.Disable) return;

        var endpoint = azureCommunicationConfig.Endpoint;
        var credential = new AzureKeyCredential(azureCommunicationConfig.AccessKey);
        var client = new EmailClient(new Uri(endpoint), credential);

        var to = GetRecipient(recipientName, recipientAddress);

        var content = new EmailContent(GetSubject(subject))
        {
            PlainText = textBody,
            Html = htmlBody
        };

        var email = new EmailMessage(emailConfig.SenderAddress, to, content)
        {
            ReplyTo = { new EmailAddress(replyTo) },
            UserEngagementTrackingDisabled = azureCommunicationConfig.UserEngagementTrackingDisabled,
        };

        foreach (var attachment in attachments)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(attachment.Filename, out var contentType);
            email.Attachments.Add(new EmailAttachment(attachment.Filename, contentType ?? "application/octet-stream", new BinaryData(attachment.Bytes)));
        }

        try
        {
            await client.SendAsync(WaitUntil.Completed, email);
        }
        catch (Exception ex)
        {
            throw new Exception($"Sending email via Azure Communication Services failed: {ex.Message}", ex);
        }
    }

    private EmailRecipients GetRecipient(string recipientName, string recipientAddress)
    {
        if (!string.IsNullOrWhiteSpace(emailConfig.RedirectAllEmailsTo))
            return new EmailRecipients([new EmailAddress(emailConfig.RedirectAllEmailsTo)]);

        return string.IsNullOrWhiteSpace(recipientName)
            ? new EmailRecipients([new EmailAddress(recipientAddress)])
            : new EmailRecipients([new EmailAddress(recipientAddress, recipientName)]);
    }

    private string GetSubject(string subject)
    {
        return (emailConfig.SubjectPrefix + subject + emailConfig.SubjectPostfix).Trim();
    }
}
