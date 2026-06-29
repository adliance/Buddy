using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid.Test;

public class MockedSendgridConfiguration : ISendgridConfiguration
{
    public string SendgridSecret => "";
    public string SendgridLabel => "Unit Tests";
}

public class MockedEmailConfiguration : IEmailConfiguration
{
    public string SenderName => "Hannes Sachsenhofer";
    public string SenderAddress => "test@adliance.net";
    public string ReplyToAddress => "hannes.sachsenhofer@adliance.net";
    public string RedirectAllEmailsTo => "";
    public string? SubjectPrefix => "[SendGrid Unit Test] ";
    public string? SubjectPostfix => null;
    public bool Disable => false;
}

public class MockedEmailAttachment(string fileName, byte[] bytes) : IEmailAttachment
{
    public string Filename { get; set; } = fileName;
    public byte[] Bytes { get; set; } = bytes;
}
