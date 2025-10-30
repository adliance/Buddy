using Adliance.AspNetCore.Buddy.Testing.Shared;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.v3;

public class BuddyFixture<TOptions, TEntryPoint> : BuddyFixtureBase<TOptions, TEntryPoint>,
    IAsyncLifetime where TOptions : BuddyFixtureOptions<TEntryPoint>, new() where TEntryPoint : class
{
    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }

    public new async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
    }
}