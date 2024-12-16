using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.InProcessNoSqlServer;

public class FixtureOptions : DefaultFixtureOptions
{
    public override string? ContentRootPath => CommonDirectoryPath.GetProjectDirectory().DirectoryPath;
}