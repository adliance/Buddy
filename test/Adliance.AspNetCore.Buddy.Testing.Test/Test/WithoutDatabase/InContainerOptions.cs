using Adliance.AspNetCore.Buddy.Testing.Containers;
using Adliance.AspNetCore.Buddy.Testing.Playwright;
using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithoutDatabase;

public class InContainerOptions : BuddyFixtureOptions<Program>
{
    public InContainerOptions()
    {
        InContainer.Add(new ContainerOptions
        {
            DockerFileDirectory = CommonDirectoryPath.GetSolutionDirectory().DirectoryPath,
            DockerFileName = "Adliance.AspNetCore.Buddy.Testing.Test.dockerfile"
        });
        
        Playwright = new PlaywrightOptions();
    }
}