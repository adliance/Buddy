using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Extensions
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddMailjetCheck(this IHealthChecksBuilder builder)
        {
            return builder.AddCheck<MailjetHealthCheck>("Mailjet");
        }
    }
}
