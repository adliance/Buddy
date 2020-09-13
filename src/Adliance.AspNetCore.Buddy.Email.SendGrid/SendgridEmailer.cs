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

        public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        {
            if (string.IsNullOrWhiteSpace(recipientAddress))
            {
                throw new ArgumentOutOfRangeException(nameof(recipientAddress));
            }

            var client = new SendGridClient(_sendgridConfig.SendgridSecret);

            var from = new EmailAddress(_emailConfig.EmailSenderAddress, _emailConfig.EmailSenderName);
            var to = new EmailAddress(recipientAddress);
            var mail = MailHelper.CreateSingleEmail(from, to, subject, textBody ?? "", htmlBody ?? "");

            if (!string.IsNullOrWhiteSpace(_emailConfig.EmailReplyToAddress))
            {
                mail.SetReplyTo(new EmailAddress(_emailConfig.EmailReplyToAddress));
            }
            
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
    }
}
