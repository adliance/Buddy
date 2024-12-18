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
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Testcontainers.MsSql;
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
    private MsSqlContainer? _dbContainer;
    private IContainer? _webContainer;
    private IPage? _page;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    protected WebApplicationFactory<TEntryPoint>? Factory { get; private set; }
    protected HttpClient Client { get; private set; } = null!;
    protected IFixtureOptions Options { get; } = new TOptions();
    protected string? DbConnectionStringInternal { get; private set; }
    protected string? DbConnectionStringExternal { get; private set; }
    protected InMemoryLogger? WebContainerLogger { get; private set; }

    protected BuddyFixture()
    {
    }

    protected BuddyFixture(WebApplicationFactory<TEntryPoint>? factory)
    {
        Factory = factory;
    }

    public async Task InitializeAsync()
    {
        await SemaphoreSlim.WaitAsync().ConfigureAwait(false);

        try
        {
            if (Options.Db != DbOptions.None) await InitDatabase();

            if (Options.WebApp == WebAppOptions.InProcess) await InitWebAppInProcess();
            else if (Options.WebApp == WebAppOptions.InContainer) await InitWebAppInContainer();
            else throw new Exception("Unsupported WebAppOption.");

            if (Options.Playwright != PlaywrightOptions.None) await InitPlaywright();
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

            if (Options.WebApp == WebAppOptions.InContainer && !string.IsNullOrWhiteSpace(DbConnectionStringInternal) && !string.IsNullOrWhiteSpace(Options.DbConnectionStringConfigurationKey))
            {
                // we set it both in ENV variable as well as configuration, becase in the app startup we may not have built the full ASP.NET configuration system, and only ENV variables are availavble
                Environment.SetEnvironmentVariable(Options.DbConnectionStringConfigurationKey, DbConnectionStringInternal);
                Options.WebAppConfiguration.TryAdd(Options.DbConnectionStringConfigurationKey, DbConnectionStringInternal);
            }
            else if (Options.WebApp == WebAppOptions.InProcess && !string.IsNullOrWhiteSpace(DbConnectionStringExternal) && !string.IsNullOrWhiteSpace(Options.DbConnectionStringConfigurationKey))
            {
                Environment.SetEnvironmentVariable(Options.DbConnectionStringConfigurationKey, DbConnectionStringExternal);
                Options.WebAppConfiguration.TryAdd(Options.DbConnectionStringConfigurationKey, DbConnectionStringExternal);
            }

            config.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(Options.WebAppConfiguration);
                configBuilder.AddEnvironmentVariables();
            });

            if (Options.ConfigureWebAppServices != null) config.ConfigureServices(Options.ConfigureWebAppServices);
            if (Options.ConfigureWebAppTestServices != null) config.ConfigureTestServices(Options.ConfigureWebAppTestServices);
        });
        await Task.CompletedTask.ConfigureAwait(false);
        Client = Factory.CreateClient();
    }

    private async Task InitWebAppInContainer()
    {
        try
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
            await InitNetwork();

            _webContainer = new ContainerBuilder()
                .WithImage(_webImage)
                .WithNetwork(_network)
                .WithPortBinding(80, true)
                .WithEnvironment(Options.DbConnectionStringConfigurationKey, DbConnectionStringInternal)
                .WithEnvironment("ASPNETCORE_URLS", "http://+")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
                .WithLogger(WebContainerLogger = new InMemoryLogger())
                .Build();
            await _webContainer.StartAsync().ConfigureAwait(false);

            var baseUri = new UriBuilder("http", _webContainer.Hostname, _webContainer.GetMappedPublicPort(80)).Uri;
            Client = new HttpClient
            {
                BaseAddress = baseUri
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to start webapp container: {ex.Message}");
        }
    }

    public IPage Page
    {
        get
        {
            if (_page == null) throw new Exception("Playwright is not initialized.");
            return _page;
        }
    }

    private async Task InitPlaywright()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Options.Playwright == PlaywrightOptions.Headless
        });
        _page = await _browser.NewPageAsync(new BrowserNewPageOptions
        {
            Locale = "en-US",
            ScreenSize = new ScreenSize
            {
                Height = 1000,
                Width = 1200
            }
        });
    }

    private async Task InitNetwork()
    {
        if (_network != null) return;
        _network = new NetworkBuilder().Build();
        await _network.CreateAsync().ConfigureAwait(false);
    }

    private async Task InitDatabase()
    {
        await InitNetwork();

        _dbContainer = new MsSqlBuilder()
            .WithNetwork(_network)
            .WithNetworkAliases("dbserver")
            .WithPortBinding(1433, true)
            .Build();
        await _dbContainer.StartAsync().ConfigureAwait(false);
        DbConnectionStringInternal = $"server=dbserver;user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database=db;encrypt=false;";
        DbConnectionStringExternal = DbConnectionStringInternal.Replace("server=dbserver", $"server=localhost,{_dbContainer.GetMappedPublicPort(1433)}");
    }

    public async Task DisposeAsync()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Client != null) Client.Dispose();

        if (_page != null) await _page.CloseAsync().ConfigureAwait(false);
        if (_browser != null) await _browser.DisposeAsync().ConfigureAwait(false);
        if (_playwright != null) _playwright.Dispose();

        if (Factory != null) await Factory.DisposeAsync().ConfigureAwait(false);
        if (_webContainer != null) await _webContainer.DisposeAsync().ConfigureAwait(false);
        if (_dbContainer != null) await _dbContainer.DisposeAsync().ConfigureAwait(false);
        if (_network != null) await _network.DisposeAsync().ConfigureAwait(false);
    }
}
