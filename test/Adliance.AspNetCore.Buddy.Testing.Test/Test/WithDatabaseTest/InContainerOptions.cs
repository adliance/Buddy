namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InContainerOptions : DefaultFixtureOptions
{
    public override WebAppOptions WebApp => WebAppOptions.InContainer;
    public override string DockerFileName => "Adliance.AspNetCore.Buddy.Testing.Test.dockerfile";
    public override PlaywrightOptions Playwright => PlaywrightOptions.Headless;
    public override DbOptions Db => DbOptions.UseSqlServer;
}