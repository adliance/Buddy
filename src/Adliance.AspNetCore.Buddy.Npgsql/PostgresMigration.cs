using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Adliance.AspNetCore.Buddy.Npgsql;

public static class PostgresMigration
{
    public static async Task Run<TContext>(TContext dbContext, PostgresDatabaseOptions options, ILogger? logger = null, CancellationToken cancellationToken = default) where TContext : DbContext
    {
        if (!options.EnableMigrations)
        {
            return;
        }

        if (!options.IsConfigured)
        {
            logger?.LogCritical("No database is configured.");
            return;
        }

        // The regular command timeout is meant for requests and is far too short for a migration.
        var previousCommandTimeout = dbContext.Database.GetCommandTimeout();
        dbContext.Database.SetCommandTimeout(options.MigrationCommandTimeoutSeconds);

        try
        {
            var waitingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (waitingMigrations.Count > 0)
            {
                logger?.LogInformation("Applying the following migrations:{NewLine}{Migrations}", Environment.NewLine, string.Join(Environment.NewLine, waitingMigrations));
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger?.LogCritical(ex, "Failed to migrate the database.");
        }
        finally
        {
            dbContext.Database.SetCommandTimeout(previousCommandTimeout);
        }
    }

    public static async Task Run<TContext, TOptions>(IServiceProvider serviceProvider, ILogger? logger = null, CancellationToken cancellationToken = default) where TContext : DbContext where TOptions : PostgresDatabaseOptions
    {
        var databaseOptions = serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;
        await Run<TContext, TOptions>(serviceProvider, databaseOptions, logger, cancellationToken);
    }

    public static async Task Run<TContext, TOptions>(IServiceProvider serviceProvider, TOptions databaseOptions, ILogger? logger = null, CancellationToken cancellationToken = default) where TContext : DbContext where TOptions : PostgresDatabaseOptions
    {
        // Failures are logged instead of thrown, so without a logger they would go unnoticed.
        logger ??= serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(typeof(PostgresMigration));

        // The context belongs to the scope, which disposes it.
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        await Run(db, databaseOptions, logger, cancellationToken);
    }
}
