using System;
using System.Collections.Generic;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Testing;

public class DefaultFixtureOptions : IFixtureOptions
{
    public virtual WebAppOptions WebApp { get; set; } = WebAppOptions.InProcess;
    public virtual string WebAppNetworkAlias { get; set; } = "webapp";
    public virtual string? ContentRootPath { get; set; }
    public virtual string? DockerFileDirectory { get; set; } = "./";
    public virtual string DockerFileName { get; set; } = "dockerfile";
    public virtual Dictionary<string, string?> WebAppConfiguration { get; set; } = new();
    public virtual Action<IServiceCollection>? ConfigureWebAppServices { get; set; }
    public virtual Action<IServiceCollection>? ConfigureWebAppTestServices { get; set; }
    public virtual PlaywrightOptions Playwright { get; set; } = PlaywrightOptions.None;
    public virtual DbOptions Db { get; set; } = DbOptions.None;
    public virtual IWaitForContainerOS? DbWaitStrategy { get; set; }
    public virtual string DbConnectionStringConfigurationKey { get; set; } = "";
    public virtual IWaitForContainerOS WebAppWaitStrategy { get; set; } = Wait.ForUnixContainer().UntilPortIsAvailable(80);
    public virtual string? LocalDbConnectionString { get; set; }

}
