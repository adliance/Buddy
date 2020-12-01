﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Adliance.Highcharts.Buddy
{
    public class HighchartsServerHealthCheck : IHealthCheck
    {
        private readonly HighchartsServer _server;

        public HighchartsServerHealthCheck(HighchartsServer server)
        {
            _server = server;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var bytes = await _server.Render(new HighchartsServerParameter
                {
                    Format = "pdf",
                    Width = 200,
                    Scale = 1,
                    Chart = new Chart(),
                    Resources = new HighchartsServerParameter.ResourcesParameter
                    {
                        Files = "https://code.highcharts.com/modules/annotations.js"
                    }
                });
                if (bytes.Length < 8000)
                {
                    throw new Exception($"Result length is only {bytes.Length} bytes.");
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Highcharts Server health check failed", ex);
            }
        }
    }
}