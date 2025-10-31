using Adliance.AspNetCore.Buddy.Testing.Shared;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing;

public class BuddyFixture<TOptions, TEntryPoint> : BuddyFixtureBase<TOptions, TEntryPoint>,
    IAsyncLifetime where TOptions : BuddyFixtureOptions<TEntryPoint>, new() where TEntryPoint : class
{

}
