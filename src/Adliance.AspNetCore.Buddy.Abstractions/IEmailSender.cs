namespace Adliance.AspNetCore.Buddy.Abstractions;

public interface IEmailSender
{
    string Name { get; }
    string EmailAddress { get; }
    string ReplyToEmailAddress { get; }
}
