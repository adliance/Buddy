using System;
using System.Threading;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Adliance.AspNetCore.Buddy.Storage
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IStorage _storage;

        public StorageHealthCheck(IStorage storage)
        {
            _storage = storage;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var path = new[] {"healthcheck", "healthcheck_file.bin"};

                await _storage.Save(new byte[] {1, 2, 3}, path);
                var bytes = await _storage.Load(path);
                await _storage.Delete(path);

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
}