namespace Adliance.AspNetCore.Buddy.Abstractions;

public interface IEmailSender
{
    string? Name { get; }
    string EmailAddress { get; }
    string? ReplyToEmailAddress { get; }
    string? ReplyToName { get; }
}

public class EmailSender : IEmailSender
{
    public string? Name { get; set; }
    public required string EmailAddress { get; set; }
    public string? ReplyToEmailAddress { get; set; }
    public string? ReplyToName { get; set; }
}
