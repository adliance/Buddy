namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InContainerOptions : DefaultFixtureOptions
{
    public override WebAppOptions WebApp => WebAppOptions.InContainer;
    public override string DockerFileName => "Adliance.AspNetCore.Buddy.Testing.Test.dockerfile";
    public override PlaywrightOptions Playwright => Environment.GetEnvironmentVariable("TF_BUILD") != null ? PlaywrightOptions.Headless : PlaywrightOptions.Headed;
    public override DbOptions Db => DbOptions.UseSqlServer;
    public override string DbConnectionStringConfigurationKey => "DatabaseConnectionString";
}