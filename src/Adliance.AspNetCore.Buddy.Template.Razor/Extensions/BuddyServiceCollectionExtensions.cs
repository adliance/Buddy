using Adliance.AspNetCore.Buddy.Abstractions;
using Adliance.AspNetCore.Buddy.Pdf;
using Adliance.AspNetCore.Buddy.Template.Razor.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddRazorTemplate(this IBuddyServiceCollection services)
        {
            services.Services.AddTransient<ITemplater, RazorTemplater>();
            services.Services.AddTransient<IPdfer, AdliancePdfer>();
            services.Services.AddTransient<IPdfRenderer, PdfRenderer>();
            services.Services.AddTransient<IEmailRenderer, EmailRenderer>();
            return services;
        }
    }
}