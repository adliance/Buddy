using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Pdf.V1;

public class PdferHealthCheck : IHealthCheck
{
    private readonly IPdfer _pdfer;
    private readonly ILogger<PdferHealthCheck> _logger;

    public PdferHealthCheck(IPdfer pdfer, ILogger<PdferHealthCheck> logger)
    {
        _pdfer = pdfer;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
    {
        var healthy = false;
        try
        {
            var bytes = await _pdfer.HtmlToPdf("This is a <b>Health</b> check", new PdfOptions());
            if (bytes.Length > 100)
            {
                healthy = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PDFer health check failed.");
        }

        if (healthy)
        {
            return await Task.FromResult(HealthCheckResult.Healthy("PDFer is healthy."));
        }

        return await Task.FromResult(HealthCheckResult.Unhealthy("PDFer is not healthy."));
    }
}
