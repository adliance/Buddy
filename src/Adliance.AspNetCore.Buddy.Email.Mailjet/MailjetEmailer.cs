using System;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Mailjet.Client;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json.Linq;
using Resources = Mailjet.Client.Resources;

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

            var client = new MailjetClient(_mailjetConfig.PublicApiKey, _mailjetConfig.PrivateApiKey)
            {
                Version = ApiVersion.V3_1
            };

            var attachmentsArray = new JArray();
            foreach (var attachment in attachments)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(attachment.Filename, out var contentType);
                attachmentsArray.Add(new JObject
                {
                    {"ContentType", contentType ?? "application/octet-stream"},
                    {"Filename", attachment.Filename},
                    {"Base64Content", Convert.ToBase64String(attachment.Bytes)}
                });
            }

            var request = new MailjetRequest
                {
                    Resource = Resources.Send.Resource
                }
                .Property(Resources.Send.Messages, new JArray
                {
                    new JObject
                    {
                        {
                            "From", new JObject
                            {
                                {"Email", _emailConfig.EmailSenderAddress},
                                {"Name", _emailConfig.EmailSenderName}
                            }
                        },
                        {
                            "To", new JArray
                            {
                                new JObject
                                {
                                    {"Email", recipientAddress}
                                }
                            }
                        },
                        {
                            "ReplyTo", string.IsNullOrWhiteSpace(_emailConfig.EmailReplyToAddress)
                                ? null
                                : new JObject
                                {
                                    {"Email", _emailConfig.EmailReplyToAddress},
                                    {"Name", _emailConfig.EmailSenderName}
                                }
                        },
                        {"CustomCampaign", string.IsNullOrWhiteSpace(_mailjetConfig.Campaign) ? null : _mailjetConfig.Campaign},
                        {"Subject", subject},
                        {"TextPart", textBody},
                        {"HTMLPart", htmlBody},
                        {"Attachments", attachmentsArray}
                    }
                });
            MailjetResponse response = await client.PostAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Sending email via Mailjet failed.{Environment.NewLine}Status code: {response.StatusCode}{Environment.NewLine}Error Info: {response.GetErrorInfo()}{Environment.NewLine}Error Message: {response.GetErrorMessage()}");
            }
        }
    }
}