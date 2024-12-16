using System;
using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing;

public interface IFixtureOptions
{
    WebAppOptions WebApp { get; }
    PlaywrightOptions Playwright { get; }
    string? ContentRootPath { get; }

    string? DockerFileDirectory { get; }
    string? DockerFileName { get; }
}

public class DefaultFixtureOptions : IFixtureOptions
{
    public virtual WebAppOptions WebApp => WebAppOptions.InProcess;
    public virtual PlaywrightOptions Playwright => PlaywrightOptions.None;
    public virtual string? ContentRootPath => null;
    public virtual string? DockerFileDirectory => CommonDirectoryPath.GetSolutionDirectory().DirectoryPath;
    public virtual string? DockerFileName => "dockerfile";
}

public enum WebAppOptions
{
    InProcess,
    InContainer
}

public enum DbOptions
{
    None,
    UseSqlServerPreferDefaultInstance,
    UseSqlServer
}

public enum PlaywrightOptions
{
    None,
    Headless,
    Headed
}
