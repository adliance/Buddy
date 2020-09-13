using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid.Extensions
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddSendgridCheck(this IHealthChecksBuilder builder)
        {
            return builder.AddCheck<SendgridHealthCheck>("SendGrid");
        }
    }
}
