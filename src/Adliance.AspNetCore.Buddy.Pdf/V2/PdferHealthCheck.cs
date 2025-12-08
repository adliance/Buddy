using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public class PdferHealthCheck(IPdferConfiguration configuration, ILogger<PdferHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(configuration.ServerUrl)) throw new Exception("No Server URL configured.");
            var endpoint = $"{configuration.ServerUrl.Trim('/')}/health";
            var response = await new HttpClient().GetAsync(endpoint, cancellationToken);
            if (response.IsSuccessStatusCode) return await Task.FromResult(HealthCheckResult.Healthy("PDFer is healthy."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PDFer health check failed.");
        }

        return await Task.FromResult(HealthCheckResult.Unhealthy("PDFer is not healthy."));
    }
}
