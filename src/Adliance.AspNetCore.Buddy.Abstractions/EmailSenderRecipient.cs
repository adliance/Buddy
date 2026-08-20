namespace Adliance.AspNetCore.Buddy.Abstractions;

public sealed class EmailSenderRecipient : IEmailSender, IEmailRecipient
{
    public required string Name { get; init; }
    public required string EmailAddress { get; set; }
    public string? ReplyToEmailAddress { get; init; }
    public string? ReplyToName { get; init; }
}
