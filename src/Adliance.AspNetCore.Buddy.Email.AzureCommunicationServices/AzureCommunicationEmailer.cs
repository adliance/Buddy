using Adliance.AspNetCore.Buddy.Abstractions;
using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.StaticFiles;

namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices;

public class AzureCommunicationEmailer(IAzureCommunicationConfiguration azureCommunicationConfig, IEmailConfiguration emailConfig) : EmailerBase(emailConfig)
{
    protected override async Task SendInternal(
        IEmailSender sender,
        IEmailRecipient[] to,
        IEmailRecipient[] cc,
        IEmailRecipient[] bcc,
        string subject,
        string htmlBody,
        string? textBody,
        params IEmailAttachment[] attachments)
    {
        var endpoint = azureCommunicationConfig.Endpoint;
        var credential = new AzureKeyCredential(azureCommunicationConfig.AccessKey);
        var client = new EmailClient(new Uri(endpoint), credential);

        var recipients = new EmailRecipients();
        foreach (var x in to) recipients.To.Add(new EmailAddress(x.EmailAddress, string.IsNullOrWhiteSpace(x.Name) ? x.EmailAddress : x.Name));
        foreach (var x in cc) recipients.CC.Add(new EmailAddress(x.EmailAddress, string.IsNullOrWhiteSpace(x.Name) ? x.EmailAddress : x.Name));
        foreach (var x in bcc) recipients.BCC.Add(new EmailAddress(x.EmailAddress, string.IsNullOrWhiteSpace(x.Name) ? x.EmailAddress : x.Name));

        var content = new EmailContent(subject)
        {
            PlainText = textBody,
            Html = htmlBody
        };

        var email = new EmailMessage(sender.EmailAddress, recipients, content)
        {
            UserEngagementTrackingDisabled = azureCommunicationConfig.UserEngagementTrackingDisabled
        };

        if (!string.IsNullOrWhiteSpace(sender.ReplyToEmailAddress))
        {
            email.ReplyTo.Add(new EmailAddress(sender.ReplyToEmailAddress, string.IsNullOrWhiteSpace(sender.ReplyToName) ? sender.ReplyToEmailAddress : sender.ReplyToName));
        }

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
}
