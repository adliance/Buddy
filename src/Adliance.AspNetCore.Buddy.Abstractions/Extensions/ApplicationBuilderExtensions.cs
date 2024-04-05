using Microsoft.AspNetCore.Builder;

namespace Adliance.AspNetCore.Buddy.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Creates and returns a new <see cref="IBuddyApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder" /> to add services to.</param>
    /// <returns>The <see cref="IBuddyApplicationBuilder" /> so that additional calls can be chained.</returns>
    public static IBuddyApplicationBuilder AddBuddy(this IApplicationBuilder builder)
    {
        return new BuddyApplicationBuilder(builder);
    }
}