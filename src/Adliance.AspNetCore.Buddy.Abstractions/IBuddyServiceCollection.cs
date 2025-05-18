using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Abstractions;

/// <summary>
/// Specifies the contract for a buddy collection of service descriptors.
/// </summary>
public interface IBuddyServiceCollection
{
    /// <summary>
    /// The collection of service descriptors.
    /// </summary>
    public IServiceCollection Services { get; }
}

/// <summary>
/// A buddy collection of service descriptors.
/// </summary>
public class BuddyServiceCollection : IBuddyServiceCollection
{
    /// <summary>
    /// Creates an instance of a buddy collection.
    /// </summary>
    /// <param name="services">A collection of service descriptors.</param>
    public BuddyServiceCollection(IServiceCollection services)
    {
        Services = services;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }
}
