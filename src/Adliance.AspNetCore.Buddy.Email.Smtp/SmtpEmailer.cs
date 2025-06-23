using System;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.StaticFiles;
using MimeKit;

namespace Adliance.AspNetCore.Buddy.Email.Smtp;

public class SmtpEmailer(ISmtpConfiguration smtpConfig, IEmailConfiguration emailConfig) : IEmailer
{
    public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
    {
        await Send(emailConfig.SenderName, emailConfig.SenderAddress, emailConfig.ReplyToAddress, "", recipientAddress, subject, htmlBody, textBody, attachments);
    }

    public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
    {
        if (string.IsNullOrWhiteSpace(recipientAddress)) throw new ArgumentOutOfRangeException(nameof(recipientAddress));

        if (emailConfig.Disable)
            return;

        var message = new MimeMessage();
        var from = new MailboxAddress(senderName, senderAddress);
        var to = GetRecipient(recipientName, recipientAddress);
        var reply = new MailboxAddress(senderName, replyTo);

        message.From.Add(from);
        message.To.Add(to);
        message.ReplyTo.Add(reply);
        message.Subject = GetSubject(subject);

        var builder = new BodyBuilder
        {
            TextBody = textBody,
            HtmlBody = htmlBody
        };

        if (attachments.Any())
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

    private MailboxAddress GetRecipient(string recipientName, string recipientAddress)
    {
        if (!string.IsNullOrWhiteSpace(emailConfig.RedirectAllEmailsTo))
            return new MailboxAddress(emailConfig.RedirectAllEmailsTo, emailConfig.RedirectAllEmailsTo);

        return new MailboxAddress(string.IsNullOrWhiteSpace(recipientName) ? recipientAddress : recipientName, recipientAddress);
    }

    private string GetSubject(string subject)
    {
        return (emailConfig.SubjectPrefix + subject + emailConfig.SubjectPostfix).Trim();
    }
}
