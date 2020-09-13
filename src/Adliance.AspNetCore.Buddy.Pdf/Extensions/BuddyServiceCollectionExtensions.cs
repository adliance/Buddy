using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Pdf.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddPdf(
            this IBuddyServiceCollection services,
            IAdliancePdferSettings pdferConfiguration)
        {
            services.Services.AddSingleton(pdferConfiguration);
            services.Services.AddTransient<IPdfer, AdliancePdfer>();
            return services;
        }
    }
}