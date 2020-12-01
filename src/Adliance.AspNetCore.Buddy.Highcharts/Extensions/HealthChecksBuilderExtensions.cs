using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Highcharts.Extensions
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddPdfCheck(this IHealthChecksBuilder builder)
        {
            return builder.AddCheck<HighchartsServerHealthCheck>("HighchartsServer");
        }
    }
}
