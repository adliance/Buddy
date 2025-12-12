using System;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.StaticFiles;
using MimeKit;

namespace Adliance.AspNetCore.Buddy.Email.Smtp;

public class SmtpEmailer(ISmtpConfiguration smtpConfig, IEmailConfiguration emailConfig) : EmailerBase(emailConfig)
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
        var message = new MimeMessage();

        foreach (var x in to) message.To.Add(GetAddress(x.Name, x.EmailAddress));
        foreach (var x in cc) message.Cc.Add(GetAddress(x.Name, x.EmailAddress));
        foreach (var x in bcc) message.Bcc.Add(GetAddress(x.Name, x.EmailAddress));

        message.From.Add(GetAddress(sender.Name, sender.EmailAddress));
        if (!string.IsNullOrWhiteSpace(sender.ReplyToEmailAddress)) message.ReplyTo.Add(GetAddress(sender.ReplyToName, sender.ReplyToEmailAddress));
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            TextBody = textBody,
            HtmlBody = htmlBody
        };

        if (attachments.Length != 0)
        {
            var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            foreach (var attachment in attachments)
            {
                fileExtensionContentTypeProvider.TryGetContentType(attachment.Filename, out var strContentType);
                ContentType.TryParse(strContentType ?? "application/octet-stream", out var contentType);
                builder.Attachments.Add(attachment.Filename, attachment.Bytes, contentType);
            }
        }

        message.Body = builder.ToMessageBody();

        // ReSharper disable once ConvertToUsingDeclaration
        using (var emailClient = new SmtpClient())
        {
            try
            {
                await emailClient.ConnectAsync(smtpConfig.Host, smtpConfig.Port);

                if (!string.IsNullOrWhiteSpace(smtpConfig.UserName))
                {
                    await emailClient.AuthenticateAsync(smtpConfig.UserName, smtpConfig.Password);
                }

                await emailClient.SendAsync(message);
                await emailClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Sending email via SMTP failed.{Environment.NewLine}: {ex.Message}", ex);
            }
        }
    }

    private static MailboxAddress GetAddress(string? recipientName, string recipientAddress)
    {
        return new MailboxAddress(string.IsNullOrWhiteSpace(recipientName) ? recipientAddress : recipientName, recipientAddress);
    }
}
