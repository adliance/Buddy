using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.StaticFiles;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet
{
    public class MailjetEmailer : IEmailer
    {
        private readonly IMailjetConfiguration _mailjetConfig;
        private readonly IEmailConfiguration _emailConfig;

        public MailjetEmailer(IMailjetConfiguration mailjetConfig, IEmailConfiguration emailConfig)
        {
            _mailjetConfig = mailjetConfig;
            _emailConfig = emailConfig;
        }

        public async Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        {
            if (string.IsNullOrWhiteSpace(recipientAddress))
            {
                throw new ArgumentOutOfRangeException(nameof(recipientAddress));
            }

            var client = new MailjetClient(_mailjetConfig.PublicApiKey, _mailjetConfig.PrivateApiKey);

            
            var email = new TransactionalEmail
            {
                From = new SendContact(_emailConfig.SenderAddress, _emailConfig.SenderName),
                Subject = subject,
                ReplyTo = new SendContact(_emailConfig.ReplyToAddress, _emailConfig.SenderName),
                CustomCampaign = _mailjetConfig.Campaign,
                To = new List<SendContact>
                {
                    new SendContact(recipientAddress)
                },
                TextPart = textBody,
                HTMLPart = htmlBody,
                Attachments = new List<Attachment>(),
            };
            
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
                throw new Exception($"Sending email via MailJet failed.{Environment.NewLine}: {ex.Message}", ex);
            }
        }
    }
}