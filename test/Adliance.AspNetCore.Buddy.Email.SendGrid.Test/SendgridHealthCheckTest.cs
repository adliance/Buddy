using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid.Test
{
    public class SendgridHealthCheckTest
    {
        [Fact]
        public async Task Health_Check_Succeeds()
        {
            var check = new SendgridHealthCheck(new SendGridConfiguration(), new NullLogger<SendgridHealthCheck>());
            Assert.Equal(HealthStatus.Healthy, (await check.CheckHealthAsync(new HealthCheckContext())).Status);
        }

        private class SendGridConfiguration : ISendgridConfiguration
        {
            public string SendgridSecret => "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";
            public string SendgridLabel => "Unit-Test";
        }
    }
}