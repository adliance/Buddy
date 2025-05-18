using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Creates and returns a new <see cref="IBuddyServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
    public static IBuddyServiceCollection AddBuddy(this IServiceCollection services)
    {
        return new BuddyServiceCollection(services);
    }
}
