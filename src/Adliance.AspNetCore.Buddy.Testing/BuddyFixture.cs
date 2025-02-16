using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Testing.Containers;
using Adliance.AspNetCore.Buddy.Testing.Database;
using Adliance.AspNetCore.Buddy.Testing.InProcess;
using Adliance.AspNetCore.Buddy.Testing.Playwright;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Microsoft.Playwright;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing;

internal static class SemaphoreHelper
{
    internal static readonly SemaphoreSlim SemaphoreSlim = new(1);
}

public class BuddyFixture<TOptions, TEntryPoint> : IAsyncLifetime where TOptions : BuddyFixtureOptions<TEntryPoint>, new() where TEntryPoint : class
{
    public INetwork? Network;
    public DatabaseResult? Database { get; set; }
    public List<ContainerResult> InContainers { get; set; } = [];
    public InProcessResult<TEntryPoint>? InProcess { get; set; }
    public PlaywrightResult? Playwright { get; set; }
    public TOptions Options { get; } = new();

    public async Task InitializeAsync()
    {
        await SemaphoreHelper.SemaphoreSlim.WaitAsync().ConfigureAwait(false);

        try
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            await BeforeInit().ConfigureAwait(false);

            await InitNetworkIfNecessary().ConfigureAwait(false);

            if (Options.Database != null)
            {
                Options.Database.Network = Network!;
                Database = await DatabaseHelper.Setup(Options.Database).ConfigureAwait(false);
            }

            await AfterDatabaseInit().ConfigureAwait(false);

            if (Options.InContainer.Any())
            {
                foreach (var o in Options.InContainer) o.Network = Network!;

                if (Database != null)
                {
                    foreach (var o in Options.InContainer)
                    {
                        if (!string.IsNullOrWhiteSpace(o.DbConnectionStringConfigurationKey)) o.Configuration.TryAdd(o.DbConnectionStringConfigurationKey, Database.DbConnectionStringInternal);
                    }
                }

                var containerTasks = Options.InContainer.Select(ContainerHelper.Setup);
                InContainers = (await Task.WhenAll(containerTasks).ConfigureAwait(false)).ToList();
            }

            if (Options.InProcess != null)
            {
                if (Database != null && !string.IsNullOrWhiteSpace(Options.InProcess.DbConnectionStringConfigurationKey))
                {
                    Options.InProcess.Configuration.TryAdd(Options.InProcess.DbConnectionStringConfigurationKey, Database.DbConnectionStringExternal);
                }

                InProcess = await InProcessHelper.Setup(Options.InProcess).ConfigureAwait(false);
            }

            if (Options.Playwright != null)
            {
                Playwright = await PlaywrightHelper.Setup(Options.Playwright).ConfigureAwait(false);
            }

            // ReSharper disable once VirtualMemberCallInConstructor
            await AfterInit().ConfigureAwait(false);
        }
        finally
        {
            SemaphoreHelper.SemaphoreSlim.Release();
        }
    }


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
    /// Is called after the initialization of database dependencies.
    /// </summary>
    protected virtual async Task AfterDatabaseInit()
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

    private async Task InitNetworkIfNecessary()
    {
        var networkIsRequired = false;
        networkIsRequired = networkIsRequired || Options.Database?.Type == DatabaseType.UseSqlServerContainer;
        networkIsRequired = networkIsRequired || Options.InContainer.Any(x => x.UseLocalAppInstead == null);
        if (!networkIsRequired) return;
        if (Network != null) return;

        var withReuse = true;
        var networkBuilder = new NetworkBuilder().WithReuse(withReuse);

        if (withReuse)
        {
            // we need to set a network name, if reuse is own, to enable reuse of the network
            networkBuilder = networkBuilder.WithName("network-with-reuse");
        }

        Network = networkBuilder.Build();
        await Network.CreateAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await BeforeDispose();

        var disposeTasks = new List<Task>();
        if (Playwright != null) disposeTasks.Add(Playwright.DisposeAsync().AsTask());
        foreach (var c in InContainers) disposeTasks.Add(c.DisposeAsync().AsTask());
        if (Network != null) disposeTasks.Add(Network.DisposeAsync().AsTask());
        if (InProcess != null) disposeTasks.Add(InProcess.DisposeAsync().AsTask());
        if (Database != null) disposeTasks.Add(Database.DisposeAsync().AsTask());
        await Task.WhenAll(disposeTasks).ConfigureAwait(false);

        await AfterDispose();
    }

    /// <summary>
    /// Convenience property that returns the HttpClient that is most likely useful.
    /// </summary>
    public HttpClient Client
    {
        get
        {
            if (InProcess != null) return InProcess.Client;
            if (InContainers.Any()) return InContainers.First().Client;
            throw new Exception("No HttpClient initialized.");
        }
    }

    /// <summary>
    /// Convenience property that returns a new Playwright page.
    /// </summary>
    public IPage Page
    {
        get
        {
            if (Playwright != null) return Playwright.Page;
            throw new Exception("No Playwright initialized.");
        }
    }
}
