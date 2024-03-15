using Adliance.AspNetCore.Buddy.Abstractions;
using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.StaticFiles;

namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices;

public class AzureCommunicationEmailer : IEmailer
{
    private readonly IAzureCommunicationConfiguration _azureCommunicationConfig;
    private readonly IEmailConfiguration _emailConfig;
    
    public AzureCommunicationEmailer(IAzureCommunicationConfiguration azureCommunicationConfig, IEmailConfiguration emailConfig)
    {
        _azureCommunicationConfig = azureCommunicationConfig;
        _emailConfig = emailConfig;
    }
    
    public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody,
        params IEmailAttachment[] attachments)
    {
        await Send(_emailConfig.SenderName, _emailConfig.SenderAddress, _emailConfig.ReplyToAddress, "", recipientAddress, subject, htmlBody, textBody, attachments);
    }

    public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress,
        string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
    {
        if (string.IsNullOrWhiteSpace(recipientAddress)) throw new ArgumentOutOfRangeException(nameof(recipientAddress));

        if (_emailConfig.Disable)
            return;
        
        string endpoint = _azureCommunicationConfig.Endpoint;
        var credential = new AzureKeyCredential(_azureCommunicationConfig.AccessKey);
        EmailClient client = new EmailClient(new Uri(endpoint), credential);

        
        var to = GetRecipient(recipientName, recipientAddress);

        var content = new EmailContent(subject)
        {
            PlainText = textBody,
            Html = htmlBody
        };

        var email = new EmailMessage(_emailConfig.SenderAddress, to, content)
        {
            ReplyTo = { new EmailAddress(replyTo) },
            UserEngagementTrackingDisabled = _azureCommunicationConfig.UserEngagementTrackingDisabled,
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
        if (!string.IsNullOrWhiteSpace(_emailConfig.RedirectAllEmailsTo))
            return new EmailRecipients([new EmailAddress(_emailConfig.RedirectAllEmailsTo)]);

        return string.IsNullOrWhiteSpace(recipientName)
            ? new EmailRecipients([new EmailAddress(recipientAddress)])
            : new EmailRecipients([new EmailAddress(recipientAddress, recipientName)]);
    }
}