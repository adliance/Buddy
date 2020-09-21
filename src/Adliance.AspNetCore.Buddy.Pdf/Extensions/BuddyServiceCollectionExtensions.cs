using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Adliance.AspNetCore.Buddy.Pdf.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddPdf(
            this IBuddyServiceCollection buddyServices,
            IPdferConfiguration pdferConfiguration)
        {
            buddyServices.Services.AddSingleton(pdferConfiguration);
            buddyServices.Services.AddTransient<IPdfer, AdliancePdfer>();
            return buddyServices;
        }

        public static IBuddyServiceCollection AddPdf(
            this IBuddyServiceCollection buddyServices,
            IConfigurationSection pdferConfigurationSection)
        {
            var pdferOptions = pdferConfigurationSection.Get<DefaultPdferConfiguration>();
            buddyServices.Services.Configure<DefaultPdferConfiguration>(pdferConfigurationSection);
            return AddPdf(buddyServices, pdferOptions);
        }
    }
}