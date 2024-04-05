using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Abstractions;

/// <summary>
/// Specifies the contract for a buddy logging builder.
/// </summary>
public interface IBuddyLoggingBuilder
{
    /// <summary>
    /// The inner logging builder.
    /// </summary>
    public ILoggingBuilder Builder { get; }
}

/// <summary>
/// A buddy logging builder.
/// </summary>
public class BuddyLoggingBuilder : IBuddyLoggingBuilder
{
    /// <summary>
    /// Creates an instance of a buddy logging builder.
    /// </summary>
    /// <param name="builder">A logging builder</param>
    public BuddyLoggingBuilder(ILoggingBuilder builder)
    {
        Builder = builder;
    }

    /// <inheritdoc />
    public ILoggingBuilder Builder { get; }
}
