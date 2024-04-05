using Microsoft.AspNetCore.Builder;

namespace Adliance.AspNetCore.Buddy.Abstractions;

/// <summary>
/// Specifies the contract for a buddy application builder.
/// </summary>
public interface IBuddyApplicationBuilder
{
    /// <summary>
    /// The inner application builder.
    /// </summary>
    public IApplicationBuilder Builder { get; }
}

/// <summary>
/// A buddy application builder.
/// </summary>
public class BuddyApplicationBuilder : IBuddyApplicationBuilder
{
    /// <summary>
    /// Creates an instance of a buddy application builder.
    /// </summary>
    /// <param name="builder">An application builder</param>
    public BuddyApplicationBuilder(IApplicationBuilder builder)
    {
        Builder = builder;
    }

    /// <inheritdoc />
    public IApplicationBuilder Builder { get; }
}
