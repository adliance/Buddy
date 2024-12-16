namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.InContainerNoSqlServer;

public class FixtureOptions : DefaultFixtureOptions
{
    public override WebAppOptions WebApp => WebAppOptions.InContainer;
    public override string? DockerFileName => "Adliance.AspNetCore.Buddy.Testing.Test.dockerfile";
}