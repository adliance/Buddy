using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithoutDatabase;

public class InContainerOptions : DefaultFixtureOptions
{
    public override string? DockerFileDirectory => CommonDirectoryPath.GetSolutionDirectory().DirectoryPath;
    public override WebAppOptions WebApp => WebAppOptions.InContainer;
    public override string DockerFileName => "Adliance.AspNetCore.Buddy.Testing.Test.dockerfile";
    public override PlaywrightOptions Playwright => PlaywrightOptions.Headless;
}