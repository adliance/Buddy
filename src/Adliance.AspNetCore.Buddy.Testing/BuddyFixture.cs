using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
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
    private IPage? _page;

    public INetwork? Network;
    public MsSqlContainer? DbContainer;
    public IContainer? WebContainer;
    public IPlaywright? Playwright;
    public IBrowser? Browser;
    public WebApplicationFactory<TEntryPoint>? Factory { get; private set; }
    public HttpClient Client { get; private set; } = null!;
    public TOptions Options { get; } = new();
    public string? DbConnectionStringInternal { get; private set; }
    public string? DbConnectionStringExternal { get; private set; }
    public InMemoryLogger? WebContainerLogger { get; private set; }
    public InMemoryLogger? DbContainerLogger { get; private set; }

    /// <summary>
    /// Is called before any initialization of the fixture dependencies (containers etc.).
    /// </summary>
    protected virtual async Task BeforeInit()
    {
        await Task.CompletedTask;
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
    /// Is called before any dependencies are being disposed.
    /// Use (override) to dispose you own dependencies that you create in, for example, AfterInit.
    /// </summary>
    protected virtual async Task BeforeDispose()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Is called after all dependencies have been disposed.
    /// </summary>
    protected virtual async Task AfterDispose()
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
            await InitNetwork();

            var containerOptions = WebContainerOptions.FromFixtureOptions("webapp", Network!, Options);
            containerOptions.Logger = new InMemoryLogger();
            if (Options.Db != DbOptions.None)
            {
                if (string.IsNullOrWhiteSpace(DbConnectionStringInternal)) throw new Exception("Unable to set connection string, as DbConnectionStringInternal is empty.");
                if (string.IsNullOrWhiteSpace(Options.DbConnectionStringConfigurationKey)) throw new Exception("Unable to set connection string, as DbConnectionStringConfigurationKey is empty.");
                containerOptions.Configuration.TryAdd(Options.DbConnectionStringConfigurationKey, DbConnectionStringInternal);
            }

            var containerResult = await WebContainerHelper.BuildAndStartWebContainer(containerOptions);
            WebContainer = containerResult.Container;
            WebContainerLogger = (InMemoryLogger)containerResult.Logger;
            Client = new HttpClient
            {
                BaseAddress = containerResult.Url
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to start webapp container: {ex.Message}");
        }
    }

    public IPage Page
    {
        get => _page ?? throw new Exception("Playwright is not initialized.");
        set => _page = value;
    }

    private async Task InitPlaywright()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        await InitNewPlaywrightBrowser();
    }

    public async Task InitNewPlaywrightBrowser()
    {
        if (Playwright == null) throw new Exception("Playwright is not initialized.");
        if (_page != null) await _page.CloseAsync().ConfigureAwait(false);
        if (Browser != null) await Browser.DisposeAsync().ConfigureAwait(false);

        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Options.Playwright == PlaywrightOptions.Headless
        });
        await InitNewPlaywrightPage();
    }

    public async Task InitNewPlaywrightPage()
    {
        if (Browser == null) throw new Exception("Browser is not initialized.");
        if (_page != null) await _page.CloseAsync().ConfigureAwait(false);

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
        if (Network != null) return;
        Network = new NetworkBuilder().Build();
        await Network.CreateAsync().ConfigureAwait(false);
    }

    private async Task InitDatabase()
    {
        await InitNetwork();

        var dbContainer = new MsSqlBuilder()
            .WithNetwork(Network)
            .WithNetworkAliases("dbserver")
            .WithLogger(DbContainerLogger = new InMemoryLogger())
            .WithPortBinding(1433, true);

        if (Options.DbWaitStrategy != null)
        {
            dbContainer = dbContainer.WithWaitStrategy(Options.DbWaitStrategy);
        }

        DbContainer = dbContainer.Build();

        await DbContainer.StartAsync().ConfigureAwait(false);
        DbConnectionStringInternal = $"server=dbserver;user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database=db;encrypt=false;";
        DbConnectionStringExternal = DbConnectionStringInternal.Replace("server=dbserver", $"server=localhost,{DbContainer.GetMappedPublicPort(1433)}");
    }

    public async Task InitializeAsync()
    {
        await SemaphoreSlim.WaitAsync().ConfigureAwait(false);

        try
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            await BeforeInit().ConfigureAwait(false);

            if (Options.Db != DbOptions.None) await InitDatabase().ConfigureAwait(false);
            if (Options.WebApp == WebAppOptions.InProcess) await InitWebAppInProcess().ConfigureAwait(false);
            else if (Options.WebApp == WebAppOptions.InContainer) await InitWebAppInContainer().ConfigureAwait(false);
            if (Options.Playwright != PlaywrightOptions.None) await InitPlaywright().ConfigureAwait(false);

            // ReSharper disable once VirtualMemberCallInConstructor
            await AfterInit().ConfigureAwait(false);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    public async Task DisposeAsync()
    {
        await BeforeDispose();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Client != null) Client.Dispose();

        if (_page != null) await _page.CloseAsync().ConfigureAwait(false);
        if (Browser != null) await Browser.DisposeAsync().ConfigureAwait(false);
        if (Playwright != null) Playwright.Dispose();

        if (Factory != null) await Factory.DisposeAsync().ConfigureAwait(false);
        if (WebContainer != null) await WebContainer.DisposeAsync().ConfigureAwait(false);
        if (DbContainer != null) await DbContainer.DisposeAsync().ConfigureAwait(false);
        if (Network != null) await Network.DisposeAsync().ConfigureAwait(false);

        await AfterDispose();
    }
}
