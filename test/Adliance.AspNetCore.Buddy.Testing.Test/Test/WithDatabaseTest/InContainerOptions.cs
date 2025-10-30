using Adliance.AspNetCore.Buddy.Testing.Shared;
using Adliance.AspNetCore.Buddy.Testing.Shared.Containers;
using Adliance.AspNetCore.Buddy.Testing.Shared.Database;
using Adliance.AspNetCore.Buddy.Testing.Shared.Playwright;
using DotNet.Testcontainers.Builders;
using Xunit;

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
