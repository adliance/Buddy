using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithoutDatabase;

public class InProcessOptions : DefaultFixtureOptions
{
    public override string? ContentRootPath => CommonDirectoryPath.GetProjectDirectory().DirectoryPath;
    public override PlaywrightOptions Playwright => PlaywrightOptions.None;
}