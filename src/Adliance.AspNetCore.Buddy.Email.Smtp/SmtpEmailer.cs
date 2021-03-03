using System;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.StaticFiles;
using MimeKit;

namespace Adliance.AspNetCore.Buddy.Email.Smtp
{
    public class SmtpEmailer : IEmailer
    {
        private readonly ISmtpConfiguration _smtpConfig;
        private readonly IEmailConfiguration _emailConfig;

        public SmtpEmailer(ISmtpConfiguration smtpConfig, IEmailConfiguration emailConfig)
        {
            _smtpConfig = smtpConfig;
            _emailConfig = emailConfig;
        }

        public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        {
            if (string.IsNullOrWhiteSpace(recipientAddress))
            {
                throw new ArgumentOutOfRangeException(nameof(recipientAddress));
            }

            var message = new MimeMessage();
            var from = new MailboxAddress(_emailConfig.SenderName, _emailConfig.SenderAddress);
            var to = new MailboxAddress(recipientAddress, recipientAddress);
            var replyTo = new MailboxAddress(_emailConfig.SenderName, _emailConfig.ReplyToAddress);

            message.From.Add(from);
            message.To.Add(to);
            message.ReplyTo.Add(replyTo);
            message.Subject = subject;

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

            using (var emailClient = new SmtpClient())
            {
                try
                {
                    await emailClient.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port);

                    if (!string.IsNullOrWhiteSpace(_smtpConfig.UserName))
                    {
                        await emailClient.AuthenticateAsync(_smtpConfig.UserName, _smtpConfig.Password);
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
    }
}