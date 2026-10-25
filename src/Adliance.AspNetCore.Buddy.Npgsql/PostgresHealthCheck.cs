using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Adliance.AspNetCore.Buddy.Npgsql;

internal sealed class PostgresHealthCheck<TContext>(TContext dbContext, bool checkPendingMigrations) : IHealthCheck where TContext : DbContext
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var failureStatus = context.Registration.FailureStatus;

        if (string.IsNullOrWhiteSpace(dbContext.Database.GetConnectionString()))
        {
            return new HealthCheckResult(failureStatus, "No database is configured.");
        }

        try
        {
            if (!await dbContext.Database.CanConnectAsync(cancellationToken))
            {
                return new HealthCheckResult(failureStatus, "Unable to connect to the database.");
            }

            // Failed migrations are only logged on startup, so this is where they become visible.
            if (checkPendingMigrations)
            {
                var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
                if (pendingMigrations.Count > 0)
                {
                    return new HealthCheckResult(
                        failureStatus,
                        $"The database has {pendingMigrations.Count} pending migration(s).",
                        data: new Dictionary<string, object> { ["PendingMigrations"] = pendingMigrations });
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new HealthCheckResult(failureStatus, "Unable to connect to the database.", ex);
        }

        return HealthCheckResult.Healthy();
    }
}
