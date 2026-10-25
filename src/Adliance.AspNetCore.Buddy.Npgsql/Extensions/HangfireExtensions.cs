using System;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Adliance.AspNetCore.Buddy.Npgsql.Extensions;

public static class HangfireExtensions
{
    extension(IGlobalConfiguration config)
    {
        public IGlobalConfiguration UsePostgresStorage<TOptions>(IServiceProvider services, PostgreSqlStorageOptions? storageOptions = null) where TOptions : PostgresDatabaseOptions
        {
            return config.UsePostgresStorage(services.GetRequiredService<IOptions<TOptions>>().Value, storageOptions);
        }

        public IGlobalConfiguration UsePostgresStorage(PostgresDatabaseOptions dbOptions, PostgreSqlStorageOptions? storageOptions = null)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(dbOptions.ConnectionString);
            if (dbOptions.IsConfigured && PostgresEntraAuthentication.IsRequired(dbOptions.ConnectionString))
            {
                PostgresEntraAuthentication.Configure(dataSourceBuilder);
            }

            var dataSource = dataSourceBuilder.Build();
            config.UsePostgreSqlStorage(x => x.UseConnectionFactory(new DataSourceConnectionFactory(dataSource)), storageOptions ?? new PostgreSqlStorageOptions());
            return config;
        }
    }

    private sealed class DataSourceConnectionFactory(NpgsqlDataSource dataSource) : IConnectionFactory
    {
        public NpgsqlConnection GetOrCreateConnection() => dataSource.CreateConnection();
    }
}
