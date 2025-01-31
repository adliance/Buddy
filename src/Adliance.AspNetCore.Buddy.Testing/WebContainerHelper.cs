using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Testing;

public static class WebContainerHelper
{
    public static async Task<WebContainerResult> BuildAndStartWebContainer(WebContainerOptions options)
    {
        var result = new WebContainerResult
        {
            Image = new DockerImage(options.Repository, "web", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture))
        };

        await new ImageFromDockerfileBuilder()
            .WithName(result.Image)
            .WithDockerfileDirectory(options.DockerFileDirectory)
            .WithDockerfile(options.DockerFileName)
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .WithDeleteIfExists(true)
            .Build()
            .CreateAsync()
            .ConfigureAwait(false);

        var webContainerBuilder = new ContainerBuilder()
            .WithImage(result.Image)
            .WithNetwork(options.Network)
            .WithNetworkAliases(options.NetworkAlias)
            .WithPortBinding(options.Port, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithLogger(result.Logger = options.Logger ?? new InMemoryLogger());

        foreach (var (key, value) in options.Configuration)
        {
            webContainerBuilder = webContainerBuilder.WithEnvironment(key, value);
        }

        if (options.WaitStrategy != null)
        {
            webContainerBuilder = webContainerBuilder.WithWaitStrategy(options.WaitStrategy);
        }

        result.Container = webContainerBuilder.Build();
        await result.Container.StartAsync().ConfigureAwait(false);
        result.Url = new UriBuilder("http", result.Container.Hostname, result.Container.GetMappedPublicPort(80)).Uri;

        return result;
    }
}

public class WebContainerOptions
{
    public static WebContainerOptions FromFixtureOptions(string repository, INetwork network, IFixtureOptions fixtureOptions)
    {
        var result = new WebContainerOptions
        {
            DockerFileName = fixtureOptions.DockerFileName ?? throw new Exception("Docker file name not specified."),
            DockerFileDirectory = fixtureOptions.DockerFileDirectory ?? throw new Exception("Docker file directory not specified."),
            WaitStrategy = fixtureOptions.WebAppWaitStrategy,
            Network = network,
            Repository = repository,
            Configuration = fixtureOptions.WebAppConfiguration,
            NetworkAlias = fixtureOptions.WebAppNetworkAlias
        };
        return result;
    }

    public required string Repository { get; set; }
    public required string DockerFileName { get; set; }
    public required string DockerFileDirectory { get; set; } = "";
    public required INetwork Network { get; set; }
    public required string NetworkAlias { get; set; }
    public Dictionary<string, string?> Configuration { get; set; } = new();
    public IWaitForContainerOS? WaitStrategy { get; set; }
    public ILogger? Logger { get; set; }
    public int Port { get; set; } = 80;
}

public class WebContainerResult : IAsyncDisposable
{
    public IImage Image { get; internal set; } = null!;
    public INetwork Network { get; internal set; } = null!;
    public IContainer Container { get; internal set; } = null!;
    public Uri Url { get; internal set; } = null!;
    public ILogger Logger { get; internal set; } = null!;

    public async ValueTask DisposeAsync()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Container != null) await Container.DisposeAsync().ConfigureAwait(false);
    }
}
