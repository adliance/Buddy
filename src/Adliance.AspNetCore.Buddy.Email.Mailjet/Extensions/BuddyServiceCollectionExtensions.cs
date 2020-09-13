using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddMailjet(
            this IBuddyServiceCollection services,
            IEmailConfiguration emailConfiguration,
            IMailjetConfiguration mailjetconfiguration)
        {
            services.Services.AddSingleton(emailConfiguration);
            services.Services.AddSingleton(mailjetconfiguration);
            services.Services.AddTransient<IEmailer, MailjetEmailer>();
            return services;
        }
    }
}