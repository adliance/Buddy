using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Test
{
    public class MockedMailjetConfiguration : IMailjetConfiguration
    {
        public string PublicApiKey => "";
        public string PrivateApiKey => "";
        public string Campaign => "Unit Tests";
    }

    public class MockedEmailConfiguration : IEmailConfiguration
    {
        public string SenderName => "Hannes Sachsenhofer";
        public string SenderAddress => "test@adliance.dev";
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