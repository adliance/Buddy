using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Adliance.AspNetCore.Buddy.Storage;

/// <summary>
/// Represents the Azure storage or local storage health check.
/// </summary>
public class StorageHealthCheck(IStorage storage) : IHealthCheck
{
    /// <inheritdoc cref="IHealthCheck.CheckHealthAsync"/>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var path = new[] { "healthcheck", "healthcheck_file.bin" };

            await storage.Save(new byte[] { 1, 2, 3 }, true, path);
            var bytes = await storage.Load(path);
            await storage.Delete(path);

            if (bytes != null && bytes.Length != 3 && bytes[0] != 1 && bytes[1] != 2 && bytes[2] != 3)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "Content does not match");
            }

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(HealthStatus.Unhealthy, null, ex);
        }
    }
}
