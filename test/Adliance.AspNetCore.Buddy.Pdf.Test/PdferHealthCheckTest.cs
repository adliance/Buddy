using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Pdf.Test
{
    public class PdferHealthCheckTest
    {
        [Fact]
        public async Task Health_Check_Succeeds()
        {
            var check = new PdferHealthCheck(new AdliancePdfer(new MockedAdliancePdferSettings()), new NullLogger<PdferHealthCheck>());
            Assert.Equal(HealthStatus.Healthy, (await check.CheckHealthAsync(new HealthCheckContext())).Status);
        }
    }
}