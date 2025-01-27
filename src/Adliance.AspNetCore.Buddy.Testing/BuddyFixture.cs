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

public class BuddyFixture<TOptions, TEntryPoint> : IClassFixture<WebApplicationFactory<TEntryPoint>>, IAsyncDisposable
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

    public IPlaywright? Playwright;
    public IBrowser? Browser;
    public WebApplicationFactory<TEntryPoint>? Factory { get; private set; }
    public HttpClient Client { get; private set; } = null!;
    public TOptions Options { get; } = new();
    public string? DbConnectionStringInternal { get; private set; }
    public string? DbConnectionStringExternal { get; private set; }
    public InMemoryLogger? WebContainerLogger { get; private set; }
    public InMemoryLogger? DbContainerLogger { get; private set; }

    protected BuddyFixture()
    {
        SemaphoreSlim.Wait();

        try
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            BeforeInit().GetAwaiter().GetResult();

            if (Options.Db != DbOptions.None) InitDatabase().GetAwaiter().GetResult();
            if (Options.WebApp == WebAppOptions.InProcess) InitWebAppInProcess().GetAwaiter().GetResult();
            else if (Options.WebApp == WebAppOptions.InContainer) InitWebAppInContainer().GetAwaiter().GetResult();
            if (Options.Playwright != PlaywrightOptions.None) InitPlaywright().GetAwaiter().GetResult();

            // ReSharper disable once VirtualMemberCallInConstructor
            AfterInit().GetAwaiter().GetResult();
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    /// <summary>
    /// Is called after the initialization of all fixture dependencies (containers etc.).
    /// Use (override) to initialize your test-specific stuff, for example a database or additional containers.
    /// </summary>
    protected virtual async Task AfterInit()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Is called before any initialization of the fixture dependencies (containers etc.).
    /// </summary>
    protected virtual async Task BeforeInit()
    {
        await Task.CompletedTask;
    }

    private async Task InitWebAppInProcess()
    {
        Factory = new WebApplicationFactory<TEntryPoint>();
        Factory = Factory.WithWebHostBuilder(config =>
        {
            if (Options.ContentRootPath != null) config.UseContentRoot(Options.ContentRootPath);

            if (Options.Db != DbOptions.None)
            {
                if (string.IsNullOrWhiteSpace(DbConnectionStringExternal)) throw new Exception("Unable to set connection string, as DbConnectionStringExternal is empty.");
                if (string.IsNullOrWhiteSpace(Options.DbConnectionStringConfigurationKey)) throw new Exception("Unable to set connection string, as DbConnectionStringConfigurationKey is empty.");
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
            _webImage = new DockerImage("localhost/" + typeof(TEntryPoint).FullName!.ToLower(), "web", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));
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

            var webContainerBuilder = new ContainerBuilder()
                .WithImage(_webImage)
                .WithNetwork(_network)
                .WithPortBinding(80, true)
                .WithEnvironment("ASPNETCORE_URLS", "http://+")
                .WithLogger(WebContainerLogger = new InMemoryLogger());

            if (!string.IsNullOrWhiteSpace(DbConnectionStringInternal))
            {
                webContainerBuilder = webContainerBuilder.WithEnvironment(Options.DbConnectionStringConfigurationKey, DbConnectionStringInternal);
            }

            if (Options.WebAppWaitStrategy != null)
            {
                webContainerBuilder = webContainerBuilder.WithWaitStrategy(Options.WebAppWaitStrategy);
            }

            _webContainer = webContainerBuilder.Build();
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
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Options.Playwright == PlaywrightOptions.Headless
        });
        _page = await Browser.NewPageAsync(new BrowserNewPageOptions
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

        var dbContainer = new MsSqlBuilder()
            .WithNetwork(_network)
            .WithNetworkAliases("dbserver")
            .WithLogger(DbContainerLogger = new InMemoryLogger())
            .WithPortBinding(1433, true);

        if (Options.DbWaitStrategy != null)
        {
            dbContainer = dbContainer.WithWaitStrategy(Options.DbWaitStrategy);
        }

        _dbContainer = dbContainer.Build();

        await _dbContainer.StartAsync().ConfigureAwait(false);
        DbConnectionStringInternal = $"server=dbserver;user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database=db;encrypt=false;";
        DbConnectionStringExternal = DbConnectionStringInternal.Replace("server=dbserver", $"server=localhost,{_dbContainer.GetMappedPublicPort(1433)}");
    }

    public async ValueTask DisposeAsync()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Client != null) Client.Dispose();

        if (_page != null) await _page.CloseAsync().ConfigureAwait(false);
        if (Browser != null) await Browser.DisposeAsync().ConfigureAwait(false);
        if (Playwright != null) Playwright.Dispose();

        if (Factory != null) await Factory.DisposeAsync().ConfigureAwait(false);
        if (_webContainer != null) await _webContainer.DisposeAsync().ConfigureAwait(false);
        if (_dbContainer != null) await _dbContainer.DisposeAsync().ConfigureAwait(false);
        if (_network != null) await _network.DisposeAsync().ConfigureAwait(false);
    }
}
