using Adliance.AspNetCore.Buddy.Testing.Shared;
using Adliance.AspNetCore.Buddy.Testing.Shared.Database;
using Adliance.AspNetCore.Buddy.Testing.Shared.InProcess;
using Adliance.AspNetCore.Buddy.Testing.Shared.Playwright;
using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.v3.Test.Test.WithDatabaseTest;

public class InProcessOptions : BuddyFixtureOptions<Program>
{
    public InProcessOptions()
    {
        InProcess = new InProcessOptions<Program>
        {
            ContentRoot = CommonDirectoryPath.GetProjectDirectory().DirectoryPath,
            DbConnectionStringConfigurationKey = "DatabaseConnectionString",
            UseKestrel = true
        };

        Playwright = new PlaywrightOptions();

        Database = new DatabaseOptions
        {
            Type = DatabaseType.UseSqlServerContainer
        };
    }
}
