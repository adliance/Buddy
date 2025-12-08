using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V2;

public class PdferHealthCheckTest
{
    [Fact]
    public async Task Health_Check_Succeeds()
    {
        var check = new PdferHealthCheck(new MockedPdferConfiguration(), new NullLogger<PdferHealthCheck>());
        Assert.Equal(HealthStatus.Healthy, (await check.CheckHealthAsync(new HealthCheckContext())).Status);
    }
}
