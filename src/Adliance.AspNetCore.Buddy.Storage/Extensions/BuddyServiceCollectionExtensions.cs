using System;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Adliance.AspNetCore.Buddy.Storage.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddStorage(
            this IBuddyServiceCollection buddyServices,
            IStorageConfiguration configuration)
        {
            buddyServices.Services.AddSingleton(configuration);

            if (configuration.UseAzureStorage)
            {
                buddyServices.Services.AddTransient<IStorage, AzureStorage>();
            }
            else if (configuration.UseLocalStorage)
            {
                buddyServices.Services.AddTransient<IStorage, LocalStorage>();
            }
            else
            {
                throw new Exception("No storage configured.");
            }

            return buddyServices;
        }

        public static IBuddyServiceCollection AddStorage(
            this IBuddyServiceCollection buddyServices,
            IConfigurationSection configurationSection)
        {
            var configuration = configurationSection.Get<DefaultStorageConfiguration>();
            buddyServices.Services.Configure<DefaultStorageConfiguration>(configurationSection);
            return AddStorage(buddyServices, configuration);
        }
    }
}