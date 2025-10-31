using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Testing.Shared;

public class InMemoryLogger : ILogger
{
    private readonly List<(LogLevel, Exception?, string)> _logLines = new();

    public IEnumerable<(LogLevel Level, Exception? Exception, string Message)> Logs => this._logLines.AsReadOnly();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        this._logLines.Add((logLevel, exception, formatter(state, exception)));
    }

    public bool IsEnabled(LogLevel logLevel) => true;
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}
