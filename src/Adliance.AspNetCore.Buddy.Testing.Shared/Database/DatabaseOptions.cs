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
    /// <summary>
    /// The default Docker image used for SQL Server.
    /// </summary>
    /// /// <seealso href="https://mcr.microsoft.com/en-us/artifact/mar/mssql/server/tags">
    /// SQL Server Container Versioning
    /// </seealso>
    public string DefaultSqlServerDockerImage { get; set; } = "mcr.microsoft.com/mssql/server:2022-latest";

    /// <summary>
    /// The default Docker image used for PostgreSQL.
    /// </summary>
    /// <seealso href="https://hub.docker.com/_/postgres">
    /// Postgres Container Versioning
    /// </seealso>
    public string DefaultPostgresDockerImage { get; set; } = "docker.io/library/postgres:18-alpine";
}

public enum DatabaseType
{
    UseSqlServerContainer,
    UseSqlServerLocal,
    UsePostgresContainer,
    UsePostgresLocal
}
