using System;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Adliance.AspNetCore.Buddy.Email.Smtp.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddSmtp(
            this IBuddyServiceCollection buddyServices,
            IEmailConfiguration emailConfiguration,
            ISmtpConfiguration smtpConfiguration)
        {
            buddyServices.Services.AddSingleton(emailConfiguration);
            buddyServices.Services.AddSingleton(smtpConfiguration);
            return AddSmtp(buddyServices);
        }

        public static IBuddyServiceCollection AddSmtp(
            this IBuddyServiceCollection buddyServices,
            IConfigurationSection emailConfigurationSection,
            IConfigurationSection smtpConfigurationSection)
        {
            var emailOptions = emailConfigurationSection.Get<DefaultEmailConfiguration>();
            buddyServices.Services.Configure<IEmailConfiguration>(emailConfigurationSection);

            var smtpOptions = smtpConfigurationSection.Get<DefaultSmtpConfiguration>();
            buddyServices.Services.Configure<ISmtpConfiguration>(smtpConfigurationSection);

            ArgumentNullException.ThrowIfNull(emailOptions);
            ArgumentNullException.ThrowIfNull(smtpOptions);
            
            return AddSmtp(buddyServices, emailOptions, smtpOptions);
        }

        public static IBuddyServiceCollection AddSmtp(
            this IBuddyServiceCollection buddyServices)
        {
            buddyServices.Services.AddTransient<IEmailer, SmtpEmailer>();
            return buddyServices;
        }
    }
}