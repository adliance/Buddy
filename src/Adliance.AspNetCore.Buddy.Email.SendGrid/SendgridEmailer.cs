using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.AspNetCore.StaticFiles;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid
{
    public class SendgridEmailer : IEmailer
    {
        private readonly ISendgridConfiguration _sendgridConfig;
        private readonly IEmailConfiguration _emailConfig;

        public SendgridEmailer(ISendgridConfiguration sendgridConfig, IEmailConfiguration emailConfig)
        {
            _sendgridConfig = sendgridConfig;
            _emailConfig = emailConfig;
        }

        public async Task Send(string recipientAddress, string subject, string htmlBody, string? textBody, params IEmailAttachment[] attachments)
        {
            await Send(_emailConfig.SenderName, _emailConfig.SenderAddress, _emailConfig.ReplyToAddress, "", recipientAddress, subject, htmlBody, textBody, attachments);
        }

        public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string? textBody, params IEmailAttachment[]? attachments)
        {
            if (string.IsNullOrWhiteSpace(recipientAddress)) throw new ArgumentOutOfRangeException(nameof(recipientAddress));

            if (_emailConfig.Disable)
                return;

            var client = new SendGridClient(_sendgridConfig.SendgridSecret);

            var from = new EmailAddress(senderAddress, senderName);
            var to = GetRecipient(recipientName, recipientAddress);
            var mail = MailHelper.CreateSingleEmail(from, to, subject, textBody, htmlBody);

            if (!string.IsNullOrWhiteSpace(replyTo)) mail.SetReplyTo(new EmailAddress(replyTo));

            mail.Categories = new List<string>
            {
                _sendgridConfig.SendgridLabel
            };

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
            if (!string.IsNullOrWhiteSpace(_emailConfig.RedirectAllEmailsTo))
                return new EmailAddress(_emailConfig.RedirectAllEmailsTo);
            
            return string.IsNullOrWhiteSpace(recipientName)
                ? new EmailAddress(recipientAddress)
                : new EmailAddress(recipientAddress, recipientName);
        }
    }
}