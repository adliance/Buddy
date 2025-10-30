using Adliance.AspNetCore.Buddy.Testing.Shared;
using Adliance.AspNetCore.Buddy.Testing.Shared.Containers;
using Adliance.AspNetCore.Buddy.Testing.Shared.Playwright;
using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.v3.Test.Test.WithoutDatabase;

public class InContainerOptions : BuddyFixtureOptions<Program>
{
    public InContainerOptions()
    {
        InContainer.Add(new ContainerOptions
        {
            DockerFileDirectory = CommonDirectoryPath.GetSolutionDirectory().DirectoryPath,
            DockerFileName = "Adliance.AspNetCore.Buddy.Testing.v3.Test.dockerfile"
        });

        Playwright = new PlaywrightOptions();
    }
}
