using System;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Test
{
    public class MockedMailjetConfiguration : IMailjetConfiguration
    {
        public string PublicApiKey => Environment.GetEnvironmentVariable("Adliance_Buddy_Tests__Mailjet_PublicApiKey") ?? throw new Exception("Environment variable \"Adliance_Buddy_Tests__Mailjet_PublicApiKey\" missing.");
        public string PrivateApiKey => Environment.GetEnvironmentVariable("Adliance_Buddy_Tests__Mailjet_PrivateApiKey") ?? throw new Exception("Environment variable \"Adliance_Buddy_Tests__Mailjet_PrivateApiKey\" missing.");
        public string Campaign => "Unit Tests";
    }

    public class MockedEmailConfiguration : IEmailConfiguration
    {
        public string SenderName => "Hannes Sachsenhofer";
        public string SenderAddress => "test@adliance.net";
        public string ReplyToAddress => "hannes.sachsenhofer@adliance.net";
        public string RedirectAllEmailsTo => "";
        public bool Disable => false;
    }

    public class MockedEmailAttachment : IEmailAttachment
    {
        public MockedEmailAttachment(string fileName, byte[] bytes)
        {
            Filename = fileName;
            Bytes = bytes;
        }
        
        public string Filename { get; set; }
        public byte[] Bytes { get; set; }
    }
}