using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Npgsql.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDatabase<T>(IConfiguration configuration) where T : DbContext
        {
            return services.AddDatabase<T>(configuration.GetSection("Database"));
        }

        public IServiceCollection AddDatabase<T>(IConfigurationSection configurationSection) where T : DbContext
        {
            return services.AddDatabase<T>(configurationSection.Get<PostgresDatabaseOptions>() ?? new PostgresDatabaseOptions());
        }

        public IServiceCollection AddDatabase<T>(string? connectionString) where T : DbContext
        {
            return services.AddDatabase<T>(new PostgresDatabaseOptions { ConnectionString = connectionString ?? string.Empty });
        }

        public IServiceCollection AddDatabase<T>(PostgresDatabaseOptions options) where T : DbContext
        {
            return services.AddDbContext<T>(dbContextOptions => dbContextOptions.UseNpgsql(options.ConnectionString, npgsql =>
            {
                npgsql.CommandTimeout(options.CommandTimeoutSeconds);

                if (options.IsConfigured && PostgresEntraAuthentication.IsRequired(options.ConnectionString))
                {
                    npgsql.ConfigureDataSource(PostgresEntraAuthentication.Configure);
                }
            }));
        }
    }
}
