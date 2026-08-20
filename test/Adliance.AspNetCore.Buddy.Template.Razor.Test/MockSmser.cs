using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

public class MockSmser : ISmser
{
    public List<(string Recipient, string Text)> SentMessages { get; } = [];

    public Exception? ExceptionToThrow { get; set; }

    public Task SendAsync(string recipient, string text)
    {
        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }

        SentMessages.Add((recipient, text));
        return Task.CompletedTask;
    }
}
