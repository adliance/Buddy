using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Database;

public class DatabaseOptions
{
    public INetwork? Network { get; set; }
    public DatabaseType Type { get; set; }
    public IWaitForContainerOS? DbWaitStrategy { get; set; }
    public string? LocalDbConnectionString { get; set; }
    public ILogger Logger { get; set; } = new InMemoryLogger();
    public string? DockerImage { get; set; }
}

public enum DatabaseType
{
    UseSqlServerContainer,
    UseSqlServerLocal,
    UsePostgresContainer,
    UsePostgresLocal
}
