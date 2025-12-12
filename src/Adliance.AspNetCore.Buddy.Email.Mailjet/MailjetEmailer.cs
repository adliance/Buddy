using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.StaticFiles;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet;

public class MailjetEmailer(IMailjetConfiguration mailjetConfig, IEmailConfiguration emailConfig) : EmailerBase(emailConfig)
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
        var client = new MailjetClient(mailjetConfig.PublicApiKey, mailjetConfig.PrivateApiKey);
        var email = new TransactionalEmail
        {
            From = GetRecipient(sender.Name, sender.EmailAddress),
            Subject = subject,
            CustomCampaign = mailjetConfig.Campaign,
            To = to.Select(x => GetRecipient(x.Name, x.EmailAddress)).ToList(),
            Cc = cc.Select(x => GetRecipient(x.Name, x.EmailAddress)).ToList(),
            Bcc = bcc.Select(x => GetRecipient(x.Name, x.EmailAddress)).ToList(),
            TextPart = textBody,
            HTMLPart = htmlBody,
            Attachments = new List<Attachment>()
        };

        if (!string.IsNullOrWhiteSpace(sender.ReplyToEmailAddress)) email.ReplyTo = GetRecipient(sender.ReplyToName, sender.EmailAddress);

        foreach (var attachment in attachments)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(attachment.Filename, out var contentType);
            email.Attachments.Add(new Attachment(attachment.Filename, contentType ?? "application/octet-stream", Convert.ToBase64String(attachment.Bytes)));
        }

        try
        {
            await client.SendTransactionalEmailAsync(email);
        }
        catch (Exception ex)
        {
            throw new Exception($"Sending email via MailJet failed: {ex.Message}", ex);
        }
    }

    private static SendContact GetRecipient(string? recipientName, string recipientAddress)
    {
        return string.IsNullOrWhiteSpace(recipientName)
            ? new SendContact(recipientAddress)
            : new SendContact(recipientAddress, recipientName);
    }
}
