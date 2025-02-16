using System;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace Adliance.AspNetCore.Buddy.Testing.Containers;

public static class ContainerHelper
{
    public static async Task<ContainerResult> Setup(ContainerOptions options)
    {
        var result = new ContainerResult
        {
            Image = new DockerImage(options.Repository, "localhost", "latest")
        };

        await new ImageFromDockerfileBuilder()
            .WithName(result.Image)
            .WithDockerfileDirectory(options.DockerFileDirectory)
            .WithDockerfile(options.DockerFileName)
            // .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .WithCleanUp(false)
            .Build()
            .CreateAsync()
            .ConfigureAwait(false);

        var webContainerBuilder = new ContainerBuilder()
            .WithImage(result.Image)
            .WithNetwork(options.Network)
            .WithNetworkAliases(options.NetworkAlias)
            .WithPortBinding(options.Port, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithReuse(true)
            .WithAutoRemove(false)
            .WithCleanUp(false)
            .WithLogger(options.Logger);

        if (options.WaitStrategy != null) webContainerBuilder = webContainerBuilder.WithWaitStrategy(options.WaitStrategy);

        foreach (var (key, value) in options.Configuration)
        {
            webContainerBuilder = webContainerBuilder.WithEnvironment(key, value);
        }

        result.Container = webContainerBuilder.Build();
        await result.Container.StartAsync().ConfigureAwait(false);
        result.Url = new UriBuilder("http", result.Container.Hostname, result.Container.GetMappedPublicPort(80)).Uri;
        result.Client = new HttpClient
        {
            BaseAddress = result.Url
        };
        return result;
    }
}
