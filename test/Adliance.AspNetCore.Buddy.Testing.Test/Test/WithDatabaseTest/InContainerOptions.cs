using Adliance.AspNetCore.Buddy.Testing.Containers;
using Adliance.AspNetCore.Buddy.Testing.Database;
using Adliance.AspNetCore.Buddy.Testing.Playwright;
using DotNet.Testcontainers.Builders;
using Xunit;
using DbType = System.Data.DbType;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InContainerOptions : BuddyFixtureOptions<Program>
{
    public InContainerOptions()
    {
        InContainer.Add(new ContainerOptions
        {
            DockerFileDirectory = CommonDirectoryPath.GetSolutionDirectory().DirectoryPath,
            DockerFileName = "Adliance.AspNetCore.Buddy.Testing.Test.dockerfile",
            DbConnectionStringConfigurationKey = "DatabaseConnectionString"
        });

        Playwright = new PlaywrightOptions();

        Database = new DatabaseOptions
        {
            Type = DatabaseType.UseSqlServerContainer
        };
    }
}



