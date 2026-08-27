using DotNet.Testcontainers.Builders;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Database;

public static class DatabaseHelper
{
    public static async Task<DatabaseResult> Setup(DatabaseOptions options)
    {
        if (options.Type == DatabaseType.UseSqlServerContainer)
        {
            var containerBuilder = new MsSqlBuilder(options.DefaultSqlServerDockerImage)
                .WithNetwork(options.Network)
                .WithNetworkAliases("db")
                .WithLogger(options.Logger)
                .WithPortBinding(1433, true)
                .WithWaitStrategy(options.DbWaitStrategy ?? Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433));

            var container = containerBuilder.Build();
            await container.StartAsync().ConfigureAwait(false);

            var dbConnectionStringInternal = $"server=db;user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database=db;encrypt=false;";
            return new DatabaseResult
            {
                MsSqlContainer = container,
                DbConnectionStringInternal = dbConnectionStringInternal,
                DbConnectionStringExternal = dbConnectionStringInternal.Replace("server=db", $"server=localhost,{container.GetMappedPublicPort(1433)}")
            };
        }

        if (options.Type == DatabaseType.UseSqlServerLocal)
        {
            if (string.IsNullOrWhiteSpace(options.LocalDbConnectionString)) throw new Exception("Unable to use local SQL Server, as setting \"LocalDbConnectionString\" is not specified.");
            return new DatabaseResult
            {
                MsSqlContainer = null,
                DbConnectionStringExternal = options.LocalDbConnectionString,
                DbConnectionStringInternal = options.LocalDbConnectionString.Replace("localhost", "host.docker.internal")
            };
        }

        if (options.Type == DatabaseType.UsePostgresContainer)
        {
            var containerBuilder = new PostgreSqlBuilder(options.DefaultPostgresDockerImage)
                .WithNetwork(options.Network)
                .WithNetworkAliases("db")
                .WithLogger(options.Logger)
                .WithPortBinding(5432, true)
                .WithWaitStrategy(options.DbWaitStrategy ?? Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(5432));

            var container = containerBuilder.Build();
            await container.StartAsync().ConfigureAwait(false);

            var dbConnectionStringInternal = $"Host=db; Database=db; Username={PostgreSqlBuilder.DefaultUsername}; Password={PostgreSqlBuilder.DefaultPassword}";
            return new DatabaseResult
            {
                PostgresContainer = container,
                DbConnectionStringInternal = dbConnectionStringInternal,
                DbConnectionStringExternal = dbConnectionStringInternal.Replace("Host=db", $"Host=localhost;Port={container.GetMappedPublicPort(5432)}")
            };
        }

        if (options.Type == DatabaseType.UsePostgresLocal)
        {
            if (string.IsNullOrWhiteSpace(options.LocalDbConnectionString)) throw new Exception("Unable to use local Postgres, as setting \"LocalDbConnectionString\" is not specified.");
            return new DatabaseResult
            {
                PostgresContainer = null,
                DbConnectionStringExternal = options.LocalDbConnectionString,
                DbConnectionStringInternal = options.LocalDbConnectionString.Replace("localhost", "host.docker.internal")
            };
        }

        throw new Exception("Unable to init database, as no database setting specified.");
    }
}
