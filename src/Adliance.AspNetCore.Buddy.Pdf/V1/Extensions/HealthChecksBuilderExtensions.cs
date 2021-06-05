using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Pdf.V1.Extensions
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddPdfCheck(this IHealthChecksBuilder builder)
        {
            return builder.AddCheck<PdferHealthCheck>("PDFer");
        }
    }
}
