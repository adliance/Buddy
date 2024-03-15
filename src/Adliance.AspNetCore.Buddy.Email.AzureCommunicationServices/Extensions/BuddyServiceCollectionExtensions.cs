using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddAzureCommunicationEmail(
            this IBuddyServiceCollection buddyServices,
            IEmailConfiguration emailConfiguration,
            IAzureCommunicationConfiguration azureCommunicationConfiguration)
        {
            buddyServices.Services.AddSingleton(emailConfiguration);
            buddyServices.Services.AddSingleton(azureCommunicationConfiguration);
            return AddAzureCommunicationEmail(buddyServices);
        }

        public static IBuddyServiceCollection AddAzureCommunicationEmail(
            this IBuddyServiceCollection buddyServices,
            IConfigurationSection emailConfigurationSection,
            IConfigurationSection azureCommunicationConfigurationSection)
        {
            var emailOptions = emailConfigurationSection.Get<DefaultEmailConfiguration>() ?? throw new Exception($"Unable to load email configuration from {emailConfigurationSection.Path}.");
            buddyServices.Services.Configure<IEmailConfiguration>(emailConfigurationSection);

            var azureCommunicationOptions = azureCommunicationConfigurationSection.Get<DefaultAzureCommunicationConfiguration>()?? throw new Exception($"Unable to load azure communication configuration from {azureCommunicationConfigurationSection.Path}.");
            buddyServices.Services.Configure<IAzureCommunicationConfiguration>(azureCommunicationConfigurationSection);

            return AddAzureCommunicationEmail(buddyServices, emailOptions, azureCommunicationOptions);
        }

        public static IBuddyServiceCollection AddAzureCommunicationEmail(
            this IBuddyServiceCollection buddyServices)
        {
            buddyServices.Services.AddTransient<IEmailer, AzureCommunicationEmailer>();
            return buddyServices;
        }
    }
}