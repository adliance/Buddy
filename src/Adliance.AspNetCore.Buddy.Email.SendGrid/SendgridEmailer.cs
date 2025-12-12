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

public class SendgridEmailer(ISendgridConfiguration sendgridConfig, IEmailConfiguration emailConfig) : EmailerBase(emailConfig)
{
    protected override async Task SendInternal(
        IEmailSender sender,
        IEmailRecipient[] to,
        IEmailRecipient[] cc,
        IEmailRecipient[] bcc,
        string subject,
        string htmlBody,
        string? textBody,
        params IEmailAttachment[]? attachments)
    {
        var client = new SendGridClient(sendgridConfig.SendgridSecret);

        var from = new EmailAddress(sender.EmailAddress, sender.Name);
        var recipient = GetRecipient(to.First().Name ?? to.First().EmailAddress, to.First().EmailAddress);
        var mail = MailHelper.CreateSingleEmail(from, recipient, subject, textBody, htmlBody);

        if (!string.IsNullOrWhiteSpace(sender.ReplyToEmailAddress)) mail.SetReplyTo(new EmailAddress(sender.ReplyToEmailAddress));
        foreach (var x in to.Skip(1)) mail.AddTo(GetRecipient(x.Name, x.EmailAddress));
        foreach (var x in cc) mail.AddCc(GetRecipient(x.Name, x.EmailAddress));
        foreach (var x in bcc) mail.AddBcc(GetRecipient(x.Name, x.EmailAddress));

        mail.Categories = [sendgridConfig.SendgridLabel];

        if (attachments != null && attachments.Length != 0)
        {
            var mailAttachments = new List<Attachment>();
            foreach (var attachment in attachments)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(attachment.Filename, out var contentType);
                contentType ??= "application/octet-stream";

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

    private static EmailAddress GetRecipient(string? recipientName, string recipientAddress)
    {
        return string.IsNullOrWhiteSpace(recipientName)
            ? new EmailAddress(recipientAddress)
            : new EmailAddress(recipientAddress, recipientName);
    }
}
