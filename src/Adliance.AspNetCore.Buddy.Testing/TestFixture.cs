using System.Globalization;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using TestsPoC.Web.Models;

namespace TestsPoC.Web.Tests;

public class TestFixture : IAsyncLifetime
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
    private IImage? _webImage;
    private INetwork? _network;
    private MsSqlContainer? _dbContainer;
    private IContainer? _webContainer;

    public HttpClient HttpClient = null!; // break nullability, makes usage in tests much easier
    public Db Db = null!; // break nullability, makes usage in tests much easier

    public virtual async Task InitializeAsync()
    {
        await SemaphoreSlim.WaitAsync().ConfigureAwait(false);

        try
        {
            _webImage = new DockerImage("localhost/testcontainers", "web", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), null);
            await new ImageFromDockerfileBuilder()
                .WithName(_webImage)
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
                .WithDockerfile("dockerfile")
                .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
                .WithDeleteIfExists(true)
                .Build()
                .CreateAsync()
                .ConfigureAwait(false);

            _network = new NetworkBuilder().Build();
            await _network.CreateAsync().ConfigureAwait(false);

            _dbContainer = new MsSqlBuilder()
                .WithNetwork(_network)
                .WithNetworkAliases("dbserver")
                .WithPortBinding(1433, true)
                .Build();
            await _dbContainer.StartAsync().ConfigureAwait(false);

            var dbConnectionString = $"server=dbserver;user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database=db;encrypt=false;";
            _webContainer = new ContainerBuilder()
                .WithImage(_webImage)
                .WithNetwork(_network)
                .WithPortBinding(80, true)
                .WithEnvironment("ASPNETCORE_URLS", "http://+")
                .WithEnvironment("DB_CONNECTIONSTRING", dbConnectionString)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
                .Build();
            await _webContainer.StartAsync().ConfigureAwait(false);

            HttpClient = new HttpClient
            {
                BaseAddress = BaseUri
            };
            dbConnectionString = dbConnectionString.Replace("server=dbserver", $"server=localhost,{_dbContainer.GetMappedPublicPort(1433)}");
            Db = new Db(new DbContextOptionsBuilder<Db>().UseSqlServer(dbConnectionString).Options);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    private Uri BaseUri
    {
        get
        {
            if (_webContainer == null) throw new Exception("WebContainer not available.");
            return new UriBuilder("http", _webContainer.Hostname, _webContainer.GetMappedPublicPort(80)).Uri;
        }
    }

    public string BaseUrl => BaseUri.ToString();

    public virtual async Task DisposeAsync()
    {
        await Db.DisposeAsync().ConfigureAwait(false);
        HttpClient.Dispose();
        if (_webContainer != null) await _webContainer.DisposeAsync().ConfigureAwait(false);
        if (_dbContainer != null) await _dbContainer.DisposeAsync().ConfigureAwait(false);
        if (_network != null) await _network.DisposeAsync().ConfigureAwait(false);
    }
}
