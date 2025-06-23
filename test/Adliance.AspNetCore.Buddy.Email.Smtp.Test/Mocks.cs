using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Email.Smtp.Test;

public class MockedSmtpConfiguration : ISmtpConfiguration
{
    public string Host => "in-v3.mailjet.com";
    public int Port => 587;
    public string UserName { get; init; } = "";
    public string Password => "";
}

public class MockedEmailConfiguration : IEmailConfiguration
{
    public string SenderName => "Hannes Sachsenhofer";
    public string SenderAddress => "office@akriva.com";
    public string ReplyToAddress => "hannes.sachsenhofer@adliance.net";
    public string RedirectAllEmailsTo => "";
    public string? SubjectPrefix => "[SMTP Unit Test] ";
    public string? SubjectPostfix => null;
    public bool Disable => false;
}

public class MockedEmailAttachment(string fileName, byte[] bytes) : IEmailAttachment
{
    public string Filename { get; set; } = fileName;
    public byte[] Bytes { get; set; } = bytes;
}
