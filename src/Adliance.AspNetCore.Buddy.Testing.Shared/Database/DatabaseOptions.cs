using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Database;

public class DatabaseOptions
{
    public INetwork? Network { get; set; }
    public DatabaseType Type { get; set; }
    public IWaitForContainerOS? DbWaitStrategy { get; set; } = Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433);
    public string? LocalDbConnectionString { get; set; }
    public ILogger Logger { get; set; } = new InMemoryLogger();
    public string DockerImage { get; set; } = "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04";
}

public enum DatabaseType
{
    UseSqlServerContainer,
    UseSqlServerLocal
}
