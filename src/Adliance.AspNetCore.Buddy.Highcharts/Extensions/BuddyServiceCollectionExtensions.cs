using System;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Adliance.AspNetCore.Buddy.Highcharts.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddHighchartsServer(
            this IBuddyServiceCollection buddyServices,
            IHighchartsServerSettings configuration)
        {
            buddyServices.Services.AddSingleton(configuration);
            return AddHighchartsServer(buddyServices);
        }

        public static IBuddyServiceCollection AddHighchartsServer(
            this IBuddyServiceCollection buddyServices,
            IConfigurationSection configurationSection)
        {
            var options = configurationSection.Get<HighchartsServerDefaultSettings>();
            buddyServices.Services.Configure<HighchartsServerDefaultSettings>(configurationSection);
            ArgumentNullException.ThrowIfNull(options);
            return AddHighchartsServer(buddyServices, options);
        }

        public static IBuddyServiceCollection AddHighchartsServer(
            this IBuddyServiceCollection buddyServices)
        {
            buddyServices.Services.AddTransient<HighchartsServer>();
            return buddyServices;
        }
    }
}