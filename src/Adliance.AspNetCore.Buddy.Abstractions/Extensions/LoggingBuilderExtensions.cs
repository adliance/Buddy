using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="ILoggingBuilder"/>.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Creates and returns a new <see cref="IBuddyLoggingBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to add services to.</param>
    /// <returns>The <see cref="IBuddyLoggingBuilder" /> so that additional calls can be chained.</returns>
    public static IBuddyLoggingBuilder AddBuddy(this ILoggingBuilder builder)
    {
        return new BuddyLoggingBuilder(builder);
    }
}