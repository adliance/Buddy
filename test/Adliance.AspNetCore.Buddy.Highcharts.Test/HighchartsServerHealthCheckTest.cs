using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Highcharts.Test
{
    public class HighchartsServerHealthCheckTest
    {
        [Fact]
        public async Task Can_Run_HealthCheck()
        {
            var service = new HighchartsServer(new MockedHighchartsServerSettings());
            var healthCheck = new HighchartsServerHealthCheck(service);
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
            Assert.True(HealthStatus.Healthy == result.Status, $"Health check status is {result.Status} ({result.Description} / {result.Exception?.Message}).");
        }
    }
}