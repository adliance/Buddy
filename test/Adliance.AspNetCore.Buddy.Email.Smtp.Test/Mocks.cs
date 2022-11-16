using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Email.Smtp.Test
{
    public class MockedSmtpConfiguration : ISmtpConfiguration
    {
        public string Host => "in-v3.mailjet.com";
        public int Port => 587;
        public string UserName { get; set; }  = "";
        public string Password => "";
    }

    public class MockedEmailConfiguration : IEmailConfiguration
    {
        public string SenderName => "Hannes Sachsenhofer";
        public string SenderAddress => "office@akriva.com";
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