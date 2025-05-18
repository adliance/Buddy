using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Test;

public class MailjetHealthCheckTest
{
    [Fact]
    public async Task Health_Check_Succeeds()
    {
        var check = new MailjetHealthCheck(new MockedMailjetConfiguration(), new MockedEmailConfiguration(), NullLogger<MailjetHealthCheck>.Instance);
        Assert.Equal(HealthStatus.Healthy, (await check.CheckHealthAsync(new HealthCheckContext())).Status);
    }
}