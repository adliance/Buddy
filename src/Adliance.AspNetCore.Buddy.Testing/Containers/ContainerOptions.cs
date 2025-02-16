using System.Collections.Generic;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Testing.Containers;

public class ContainerOptions
{
    public static ContainerOptions Clone(string repository, ContainerOptions options)
    {
        var result = new ContainerOptions
        {
            Repository = repository,
            DockerFileName = options.DockerFileName,
            DockerFileDirectory = options.DockerFileDirectory,
            WaitStrategy = options.WaitStrategy,
            Network = options.Network,
            Configuration = options.Configuration,
            NetworkAlias = options.NetworkAlias,
            Port = options.Port,
            Logger = options.Logger
        };
        return result;
    }

    public INetwork? Network { get; set; }
    public string Repository { get; set; } = "webapp";
    public string DockerFileName { get; set; } = "dockerfile";
    public string DockerFileDirectory { get; set; } = "./";
    public string NetworkAlias { get; set; } = "webapp";
    public Dictionary<string, string?> Configuration { get; set; } = new();
    public IWaitForContainerOS? WaitStrategy { get; set; } = Wait.ForUnixContainer().UntilPortIsAvailable(80);
    public int Port { get; set; } = 80;
    public ILogger Logger { get; set; } = new InMemoryLogger();
    public string DbConnectionStringConfigurationKey { get; set; } = "";
}
