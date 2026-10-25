using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Adliance.AspNetCore.Buddy.Npgsql.Extensions;

public static class HealthChecksBuilderExtensions
{
    extension(IHealthChecksBuilder builder)
    {
        /// <summary>
        /// Adds a health check that reports the database as failed when no connection string is configured,
        /// the database cannot be reached or, if <paramref name="checkPendingMigrations"/> is set, migrations are pending.
        /// </summary>
        public IHealthChecksBuilder AddPostgresCheck<T>(
            string? name = null,
            HealthStatus? failureStatus = null,
            IEnumerable<string>? tags = null,
            TimeSpan? timeout = null,
            bool checkPendingMigrations = true) where T : DbContext
        {
            return builder.Add(new HealthCheckRegistration(
                name ?? typeof(T).Name,
                serviceProvider => new PostgresHealthCheck<T>(serviceProvider.GetRequiredService<T>(), checkPendingMigrations),
                failureStatus,
                tags,
                timeout));
        }
    }
}
