using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.AspNetCore.StaticFiles;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid;

public class SendgridEmailer(ISendgridConfiguration sendgridConfig, IEmailConfiguration emailConfig) : IEmailer
{
    public async Task Send(string recipientAddress, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
    {
        await Send(emailConfig.SenderName, emailConfig.SenderAddress, emailConfig.ReplyToAddress, "", recipientAddress, subject, htmlBody, textBody, attachments);
    }

    public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string? textBody, params IEmailAttachment[]? attachments)
    {
        if (string.IsNullOrWhiteSpace(recipientAddress)) throw new ArgumentOutOfRangeException(nameof(recipientAddress));

        if (emailConfig.Disable)
            return;

        var client = new SendGridClient(sendgridConfig.SendgridSecret);

        var from = new EmailAddress(senderAddress, senderName);
        var to = GetRecipient(recipientName, recipientAddress);
        var mail = MailHelper.CreateSingleEmail(from, to, GetSubject(subject), textBody, htmlBody);

        if (!string.IsNullOrWhiteSpace(replyTo)) mail.SetReplyTo(new EmailAddress(replyTo));

        mail.Categories = [sendgridConfig.SendgridLabel];

        if (attachments != null && attachments.Any())
        {
            var mailAttachments = new List<Attachment>();
            foreach (var attachment in attachments)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(attachment.Filename, out var contentType);
                contentType = contentType ?? "application/octet-stream";

                mailAttachments.Add(new Attachment
                {
                    Filename = attachment.Filename,
                    Content = Convert.ToBase64String(attachment.Bytes),
                    Type = contentType
                });
            }

            mail.Attachments = mailAttachments;
        }

        var response = await client.SendEmailAsync(mail);
        if (response.StatusCode != HttpStatusCode.Accepted)
        {
            var responseBody = await response.Body.ReadAsStringAsync();
            throw new Exception($"Sending email via SendGrid failed.{Environment.NewLine}Status code: {response.StatusCode}{Environment.NewLine}Body: {responseBody}");
        }
    }

    private EmailAddress GetRecipient(string recipientName, string recipientAddress)
    {
        if (!string.IsNullOrWhiteSpace(emailConfig.RedirectAllEmailsTo))
            return new EmailAddress(emailConfig.RedirectAllEmailsTo);

        return string.IsNullOrWhiteSpace(recipientName)
            ? new EmailAddress(recipientAddress)
            : new EmailAddress(recipientAddress, recipientName);
    }

    private string GetSubject(string subject)
    {
        return (emailConfig.SubjectPrefix + subject + emailConfig.SubjectPostfix).Trim();
    }
}
