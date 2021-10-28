using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IBuddyServiceCollection"/>.
    /// </summary>
    public static class BuddyServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the razor templater to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
        public static IBuddyServiceCollection AddRazorTemplater(this IBuddyServiceCollection services)
        {
            services.Services.AddTransient<ITemplater, RazorTemplater>();
            return services;
        }

        /// <summary>
        /// Adds the razor templater and the PDF V1 renderer to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
        public static IBuddyServiceCollection AddRazorPdfV1Renderer(this IBuddyServiceCollection services)
        {
            services.Services.AddTransient<ITemplater, RazorTemplater>();
            services.Services.AddTransient<V1.IPdfRenderer, V1.PdfRenderer>();
            return services;
        }

        /// <summary>
        /// Adds the razor templater and the PDF V2 renderer to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
        public static IBuddyServiceCollection AddRazorPdfV2Renderer(this IBuddyServiceCollection services)
        {
            services.Services.AddTransient<ITemplater, RazorTemplater>();
            services.Services.AddTransient<V2.IPdfRenderer, V2.PdfRenderer>();
            return services;
        }

        /// <summary>
        /// Adds the razor templater and the email renderer to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
        public static IBuddyServiceCollection AddRazorEmailRenderer(this IBuddyServiceCollection services)
        {
            services.Services.AddTransient<ITemplater, RazorTemplater>();
            services.Services.AddTransient<IEmailRenderer, EmailRenderer>();
            return services;
        }
        
        /// <summary>
        /// Adds the razor templater and the SMS renderer to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
        public static IBuddyServiceCollection AddRazorSmsRenderer(this IBuddyServiceCollection services)
        {
            services.AddRazorTemplater();
            services.Services.AddTransient<ISmsRenderer, SmsRenderer>();
            return services;
        }
    }
}
