using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddMailjet(
            this IBuddyServiceCollection buddyServices,
            IEmailConfiguration emailConfiguration,
            IMailjetConfiguration mailjetconfiguration)
        {
            buddyServices.Services.AddSingleton(emailConfiguration);
            buddyServices.Services.AddSingleton(mailjetconfiguration);
            buddyServices.Services.AddTransient<IEmailer, MailjetEmailer>();
            return buddyServices;
        }
        
        public static IBuddyServiceCollection AddMailjet(
            this IBuddyServiceCollection buddyServices,
            IConfigurationSection emailConfigurationSection,
            IConfigurationSection mailjetConfigurationSection)
        {
            var emailOptions = emailConfigurationSection.Get<DefaultEmailConfiguration>();
            buddyServices.Services.Configure<IEmailConfiguration>(emailConfigurationSection);
            
            var mailjetOptions = mailjetConfigurationSection.Get<DefaultMailjetConfiguration>();
            buddyServices.Services.Configure<IMailjetConfiguration>(mailjetConfigurationSection);

            return AddMailjet(buddyServices, emailOptions, mailjetOptions);
        }
    }
}