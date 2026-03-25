using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Containers;

public class ContainerOptions
{
    /// <summary>
    /// The docker network to use. If null, a new network will be created.
    /// </summary>
    public INetwork? Network { get; set; }

    /// <summary>
    /// A custom image to use. Might be a DockerHub image or a local image.
    /// </summary>
    public DockerImage? Image { get; set; }

    /// <summary>
    /// The Image name used when building the image from a dockerfile. In that case, the registry is always localhost;
    /// so the resulting full image name is localhost/[repository]:latest.
    /// </summary>
    public string Repository { get; set; } = "webapp";

    /// <summary>
    /// The name of the dockerfile to use. Only used if <see cref="Image"/> is null.
    /// </summary>
    public string DockerFileName { get; set; } = "dockerfile";

    /// <summary>
    /// The location of the dockerfile to use. Only used if <see cref="Image"/> is null.
    /// </summary>
    public string DockerFileDirectory { get; set; } = "./";

    /// <summary>
    /// The network alias under which the container is reachable.
    /// </summary>
    public string NetworkAlias { get; set; } = "webapp";

    /// <summary>
    /// A dictionary of environment variables to set in the container.
    /// </summary>
    public Dictionary<string, string?> Configuration { get; set; } = new();

    /// <summary>
    /// The wait strategy to determine when the container is ready.
    /// By default, the container is ready when the internal TCP port is available.
    /// </summary>
    public IWaitForContainerOS? WaitStrategy
    {
        get => field ?? Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(Port);
        set;
    }

    /// <summary>
    /// The port on which the service in the container is listening to.
    /// </summary>
    public int Port { get; set; } = 80;

    /// <summary>
    /// A logger to direct the output of the container to.
    /// </summary>
    public ILogger Logger { get; set; } = new InMemoryLogger();

    /// <summary>
    /// The configuration key under which the database connection string is stored.
    /// Will be dynamically added to <see cref="Configuration"/> if a <see cref="BuddyFixtureOptions{TEntryPoint}.Database"/> is configured.
    /// </summary>
    public string DbConnectionStringConfigurationKey { get; set; } = "";

    /// <summary>
    /// A function to configure the container builder before the container is started.
    /// </summary>
    public Func<ContainerBuilder, ContainerBuilder>? ConfigureContainer { get; set; }

    /// <summary>
    /// The timeout for HTTP requests to the container.
    /// </summary>
    public TimeSpan ClientTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Set this to a local URL (eg. of an already running web app); if this value is set, then no container will be started.
    /// This is useful for local debugging scenarios.
    /// </summary>
    public Uri? UseLocalAppInstead { get; set; }
}
