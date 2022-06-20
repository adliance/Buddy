using System;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using MailKit.Net.Smtp;
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
            await Send(_emailConfig.SenderName, _emailConfig.SenderAddress, _emailConfig.ReplyToAddress, "", recipientAddress, subject, htmlBody, textBody, attachments);
        }

        public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        {
            if (string.IsNullOrWhiteSpace(recipientAddress)) throw new ArgumentOutOfRangeException(nameof(recipientAddress));

            var message = new MimeMessage();
            var from = new MailboxAddress(senderName, senderAddress);
            var to = new MailboxAddress(string.IsNullOrWhiteSpace(recipientName) ? recipientAddress : recipientName, recipientAddress);
            var reply = new MailboxAddress(senderName, replyTo);

            message.From.Add(from);
            message.To.Add(to);
            message.ReplyTo.Add(reply);
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

            // ReSharper disable once ConvertToUsingDeclaration
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