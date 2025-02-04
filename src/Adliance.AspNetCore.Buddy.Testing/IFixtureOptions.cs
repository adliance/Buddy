using System;
using System.Collections.Generic;
using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Testing;

public interface IFixtureOptions
{
    WebAppOptions WebApp { get; }
    IWaitForContainerOS? WebAppWaitStrategy { get; }
    string WebAppNetworkAlias { get; }
    string? ContentRootPath { get; }
    string? DockerFileDirectory { get; }
    string? DockerFileName { get; }
    Dictionary<string, string?> WebAppConfiguration { get; }
    Action<IServiceCollection>? ConfigureWebAppServices { get; }
    Action<IServiceCollection>? ConfigureWebAppTestServices { get; }

    PlaywrightOptions Playwright { get; }
    DbOptions Db { get; }
    IWaitForContainerOS? DbWaitStrategy { get; }
    string? DbConnectionStringConfigurationKey { get; }
    string? LocalDbConnectionString { get; }
}

public enum WebAppOptions
{
    InProcess,
    InContainer
}

public enum DbOptions
{
    None,
    UseSqlServerContainer,
    UseSqlServerLocal
}

public enum PlaywrightOptions
{
    None,
    Headless,
    Headed
}
