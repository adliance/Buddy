using System;
using System.Collections.Generic;
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
            await Send(_emailConfig.SenderName, _emailConfig.SenderAddress, _emailConfig.ReplyToAddress, "", recipientAddress, subject, htmlBody, textBody, attachments);
        }

        public async Task Send(string senderName, string senderAddress, string replyTo, string recipientName, string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments)
        {
            if (string.IsNullOrWhiteSpace(recipientAddress)) throw new ArgumentOutOfRangeException(nameof(recipientAddress));

            var client = new MailjetClient(_mailjetConfig.PublicApiKey, _mailjetConfig.PrivateApiKey);

            var to = string.IsNullOrWhiteSpace(recipientName) ? new SendContact(recipientAddress) : new SendContact(recipientAddress, recipientName);
            var email = new TransactionalEmail
            {
                From = new SendContact(senderAddress, senderName),
                Subject = subject,
                ReplyTo = new SendContact(replyTo, senderName),
                CustomCampaign = _mailjetConfig.Campaign,
                To = new List<SendContact> { to },
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