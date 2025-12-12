namespace Adliance.AspNetCore.Buddy.Abstractions;

public interface IEmailRecipient
{
    string Name { get; }
    string EmailAddress { get; }
}
