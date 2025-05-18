using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid.Test;

public class SendgridHealthCheckTest
{
    [Fact(Skip = "No API key configured.")]
    public async Task Health_Check_Succeeds()
    {
        var check = new SendgridHealthCheck(new SendGridConfiguration(), new NullLogger<SendgridHealthCheck>());
        Assert.Equal(HealthStatus.Healthy, (await check.CheckHealthAsync(new HealthCheckContext())).Status);
    }

    private sealed class SendGridConfiguration : ISendgridConfiguration
    {
        public string SendgridSecret => "";
        public string SendgridLabel => "Unit-Test";
    }
}
