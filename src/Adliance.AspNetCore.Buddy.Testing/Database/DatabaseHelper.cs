using System;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace Adliance.AspNetCore.Buddy.Testing.Database;

public static class DatabaseHelper
{
    public static async Task<DatabaseResult> Setup(DatabaseOptions options)
    {
        if (options.Type == DatabaseType.UseSqlServerContainer)
        {
            var containerBuilder = new MsSqlBuilder()
                .WithNetwork(options.Network)
                .WithNetworkAliases("db")
                .WithLogger(options.Logger)
                .WithReuse(true)
                .WithAutoRemove(false)
                .WithCleanUp(false)
                .WithPortBinding(1433, true);

            if (options.DbWaitStrategy != null)
            {
                containerBuilder = containerBuilder.WithWaitStrategy(options.DbWaitStrategy);
            }

            var container = containerBuilder.Build();
            await container.StartAsync().ConfigureAwait(false);

            var dbConnectionStringInternal = $"server=db;user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database=db;encrypt=false;";
            return new DatabaseResult
            {
                Container = container,
                DbConnectionStringInternal = dbConnectionStringInternal,
                DbConnectionStringExternal = dbConnectionStringInternal.Replace("server=db", $"server=localhost,{container.GetMappedPublicPort(1433)}")
            };
        }

        if (options.Type == DatabaseType.UseSqlServerLocal)
        {
            if (string.IsNullOrWhiteSpace(options.LocalDbConnectionString)) throw new Exception("Unable to use local SQL Server, as setting \"LocalDbConnectionString\" is not specified.");
            return new DatabaseResult
            {
                Container = null,
                DbConnectionStringExternal = options.LocalDbConnectionString,
                DbConnectionStringInternal = options.LocalDbConnectionString.Replace("localhost", "host.docker.internal")
            };
        }

        throw new Exception("Unable to init database, as no database setting specified.");
    }
}
