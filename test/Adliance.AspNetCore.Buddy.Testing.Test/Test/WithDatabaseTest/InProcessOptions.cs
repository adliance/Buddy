using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InProcessOptions : DefaultFixtureOptions
{
    public override string? ContentRootPath => CommonDirectoryPath.GetProjectDirectory().DirectoryPath;
    public override DbOptions Db => DbOptions.UseSqlServerContainer;
    public override string DbConnectionStringConfigurationKey => "DatabaseConnectionString";
}