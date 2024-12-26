using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InProcessOptions : DefaultFixtureOptions
{
    public override string? ContentRootPath => CommonDirectoryPath.GetProjectDirectory().DirectoryPath;
    public override DbOptions Db => DbOptions.UseSqlServer;
    public override string DbConnectionStringConfigurationKey => "DatabaseConnectionString";
}