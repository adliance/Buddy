using System;
using System.Collections.Generic;
using Adliance.AspNetCore.Buddy.Testing.Containers;
using Adliance.AspNetCore.Buddy.Testing.Database;
using Adliance.AspNetCore.Buddy.Testing.InProcess;
using Adliance.AspNetCore.Buddy.Testing.Playwright;

namespace Adliance.AspNetCore.Buddy.Testing;

public class BuddyFixtureOptions<TEntryPoint> where TEntryPoint : class
{
    public InProcessOptions<TEntryPoint>? InProcess { get; set; }
    public DatabaseOptions? Database { get; set; }
    public List<ContainerOptions> InContainer { get; set; } = new();
    public PlaywrightOptions? Playwright { get; set; }
}
