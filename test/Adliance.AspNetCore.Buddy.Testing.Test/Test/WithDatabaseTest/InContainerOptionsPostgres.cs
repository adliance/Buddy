using Adliance.AspNetCore.Buddy.Testing.Shared;
using Adliance.AspNetCore.Buddy.Testing.Shared.Containers;
using Adliance.AspNetCore.Buddy.Testing.Shared.Database;
using Adliance.AspNetCore.Buddy.Testing.Shared.Playwright;
using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InContainerOptionsPostgres : BuddyFixtureOptions<Program>
{
    public InContainerOptionsPostgres()
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
            Type = DatabaseType.UsePostgresContainer
        };
    }
}
