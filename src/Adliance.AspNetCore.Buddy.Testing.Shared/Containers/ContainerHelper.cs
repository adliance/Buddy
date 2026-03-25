using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Containers;

public static class ContainerHelper
{
    public static async Task<ContainerResult> Setup(ContainerOptions options)
    {
        if (options.UseLocalAppInstead != null)
        {
            return new ContainerResult
            {
                Url = options.UseLocalAppInstead,
                Client = new HttpClient
                {
                    BaseAddress = options.UseLocalAppInstead,
                    Timeout = options.ClientTimeout
                }
            };
        }

        var result = new ContainerResult
        {
            Image = options.Image ?? new DockerImage(options.Repository, "localhost", "latest")
        };

        if (options.Image == null)
        {
            await new ImageFromDockerfileBuilder()
                .WithName(result.Image)
                .WithDockerfileDirectory(options.DockerFileDirectory)
                .WithDockerfile(options.DockerFileName)
                .Build()
                .CreateAsync()
                .ConfigureAwait(false);
        }

        var webContainerBuilder = new ContainerBuilder(result.Image)
            .WithNetwork(options.Network)
            .WithNetworkAliases(options.NetworkAlias)
            .WithPortBinding(options.Port, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+")
            .WithLogger(options.Logger);

        if (options.WaitStrategy != null) webContainerBuilder = webContainerBuilder.WithWaitStrategy(options.WaitStrategy);

        foreach (var (key, value) in options.Configuration)
        {
            webContainerBuilder = webContainerBuilder.WithEnvironment(key, value);
        }

        if (options.ConfigureContainer != null) webContainerBuilder = options.ConfigureContainer.Invoke(webContainerBuilder);

        result.Container = webContainerBuilder.Build();
        await result.Container.StartAsync().ConfigureAwait(false);
        result.Url = new UriBuilder("http", result.Container.Hostname, result.Container.GetMappedPublicPort(options.Port)).Uri;
        result.Client = new HttpClient
        {
            BaseAddress = result.Url,
            Timeout = options.ClientTimeout
        };
        return result;
    }
}
