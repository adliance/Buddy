using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing;

public class BuddyFixture<TOptions, TEntryPoint> : IClassFixture<WebApplicationFactory<TEntryPoint>>, IAsyncLifetime
    where TEntryPoint : class
    where TOptions : IFixtureOptions, new()
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
    private IImage? _webImage;
    private INetwork? _network;
    private IContainer? _webContainer;

    protected WebApplicationFactory<TEntryPoint>? Factory;
    protected HttpClient Client = null!;
    protected IFixtureOptions Options { get; } = new TOptions();

    protected BuddyFixture()
    {
    }

    protected BuddyFixture(WebApplicationFactory<TEntryPoint> factory)
    {
        Factory = factory;
    }

    public async Task InitializeAsync()
    {
        await SemaphoreSlim.WaitAsync().ConfigureAwait(false);

        try
        {
            if (Options.WebApp == WebAppOptions.InProcess) await InitWebAppInProcess();
            else if (Options.WebApp == WebAppOptions.InContainer) await InitWebAppInContainer();
            else throw new Exception("Unsupported WebAppOption.");
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }


    private async Task InitWebAppInProcess()
    {
        if (Factory == null) throw new Exception("WebApplicationFactory was not initialized.");

        Factory = Factory.WithWebHostBuilder(config =>
        {
            if (Options.ContentRootPath != null) config.UseContentRoot(Options.ContentRootPath);

            /*config.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["BackgroundJobs:Enable"] = "false",
                    ["DatabaseConnectionString"] = "Data Source=localhost; Initial Catalog=raptorserver-unittests; User ID=sa; Password=P4ss.W0rd; MultipleActiveResultSets=False; Encrypt=false;"
                });
                configBuilder.AddEnvironmentVariables();
            });

            config.ConfigureServices(services => { services.AddSingleton(_ => Mock.Of<ICacheService>()); });*/
        });
        await Task.CompletedTask.ConfigureAwait(false);
        Client = Factory.CreateClient();
    }

    private async Task InitWebAppInContainer()
    {
        _webImage = new DockerImage("localhost/" + typeof(TEntryPoint).FullName!.ToLower(), "web", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), null);
        await new ImageFromDockerfileBuilder()
            .WithName(_webImage)
            .WithDockerfileDirectory(Options.DockerFileDirectory)
            .WithDockerfile(Options.DockerFileName)
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .WithDeleteIfExists(true)
            .Build()
            .CreateAsync()
            .ConfigureAwait(false);

        _network = new NetworkBuilder().Build();
        await _network.CreateAsync().ConfigureAwait(false);
        _webContainer = new ContainerBuilder()
            .WithImage(_webImage)
            .WithNetwork(_network)
            .WithPortBinding(80, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
            .Build();
        await _webContainer.StartAsync().ConfigureAwait(false);

        var baseUri = new UriBuilder("http", _webContainer.Hostname, _webContainer.GetMappedPublicPort(80)).Uri;
        Client = new HttpClient
        {
            BaseAddress = baseUri
        };
    }


    public async Task DisposeAsync()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Client != null) Client.Dispose();
        if (Factory != null) await Factory.DisposeAsync().ConfigureAwait(false);
        if (_webContainer != null) await _webContainer.DisposeAsync().ConfigureAwait(false);
        if (_network != null) await _network.DisposeAsync().ConfigureAwait(false);
    }
}
