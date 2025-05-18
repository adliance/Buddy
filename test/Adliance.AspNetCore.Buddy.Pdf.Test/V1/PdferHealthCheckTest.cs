using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V1;

public class PdferHealthCheckTest
{
    [Fact]
    public async Task Health_Check_Succeeds()
    {
        var check = new PdferHealthCheck(new AdliancePdfer(new MockedPdferConfiguration()), new NullLogger<PdferHealthCheck>());
        Assert.Equal(HealthStatus.Healthy, (await check.CheckHealthAsync(new HealthCheckContext())).Status);
    }
}
