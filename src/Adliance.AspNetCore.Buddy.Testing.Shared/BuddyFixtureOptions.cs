using Adliance.AspNetCore.Buddy.Testing.Shared.Containers;
using Adliance.AspNetCore.Buddy.Testing.Shared.Database;
using Adliance.AspNetCore.Buddy.Testing.Shared.InProcess;
using Adliance.AspNetCore.Buddy.Testing.Shared.Playwright;

namespace Adliance.AspNetCore.Buddy.Testing.Shared;

public class BuddyFixtureOptions<TEntryPoint> where TEntryPoint : class
{
    public InProcessOptions<TEntryPoint>? InProcess { get; set; }
    public DatabaseOptions? Database { get; set; }
    public List<ContainerOptions> InContainer { get; set; } = new();
    public PlaywrightOptions? Playwright { get; set; }
}
