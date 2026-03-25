using Adliance.AspNetCore.Buddy.Testing.Shared;
using Adliance.AspNetCore.Buddy.Testing.Shared.Containers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;

namespace Adliance.AspNetCore.Buddy.Testing.v3.Test.Test.WithMockContainer;

public class InContainerOptions : BuddyFixtureOptions<Program>
{
    public InContainerOptions()
    {
        InContainer.Add(new ContainerOptions
        {
            Image = new DockerImage("mockoon/cli:latest"),
            ConfigureContainer = builder =>
                builder
                    .WithResourceMapping(
                        Path.Combine(CommonDirectoryPath.GetProjectDirectory().DirectoryPath, "api-mock.json"),
                        "/config")
                    .WithEntrypoint("mockoon-cli", "start", "--data", "/config/api-mock.json"),
            Port = 3000
        });
    }
}