using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddRazorTemplater(this IBuddyServiceCollection services)
        {
            services.Services.AddTransient<ITemplater, RazorTemplater>();
            return services;
        }

        public static IBuddyServiceCollection AddRazorPdfRenderer(this IBuddyServiceCollection services)
        {
            services.Services.AddTransient<ITemplater, RazorTemplater>();
            services.Services.AddTransient<IPdfRenderer, PdfRenderer>();
            return services;
        }

        public static IBuddyServiceCollection AddRazorEmailRenderer(this IBuddyServiceCollection services)
        {
            services.Services.AddTransient<ITemplater, RazorTemplater>();
            services.Services.AddTransient<IEmailRenderer, EmailRenderer>();
            return services;
        }
    }
}