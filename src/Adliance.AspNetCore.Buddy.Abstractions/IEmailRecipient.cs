namespace Adliance.AspNetCore.Buddy.Abstractions;

public interface IEmailRecipient
{
    string? Name { get; }
    string EmailAddress { get; set; }
}

public class EmailRecipient : IEmailRecipient
{
    public string? Name { get; set; }
    public required string EmailAddress { get; set; }
}
