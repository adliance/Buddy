using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Test
{
    public class MockedMailjetConfiguration : IMailjetConfiguration
    {
        public string PublicApiKey => "";
        public string PrivateApiKey => "";
        public string Campaign => "igevia Unit Tests";
    }

    public class MockedEmailConfiguration : IEmailConfiguration
    {
        public string EmailSenderName => "Hannes Sachsenhofer";
        public string EmailSenderAddress => "contact@igevia.com";
        public string EmailReplyToAddress => "hannes.sachsenhofer@adliance.net";
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