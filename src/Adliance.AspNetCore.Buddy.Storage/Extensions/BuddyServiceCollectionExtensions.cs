using System;
using System.IO;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.Storage;
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

            if (configuration.ConfigureDataProtection && configuration.UseAzureStorage && !string.IsNullOrWhiteSpace(configuration.AzureStorageConnectionString) && !string.IsNullOrWhiteSpace(configuration.DataProtectionContainer))
            {
                buddyServices.Services.AddDataProtection().PersistKeysToAzureBlobStorage(CloudStorageAccount.Parse(configuration.AzureStorageConnectionString), configuration.DataProtectionContainer);
            }
            else if (configuration.ConfigureDataProtection && configuration.UseLocalStorage && !string.IsNullOrWhiteSpace(configuration.DataProtectionContainer))
            {
                var directory = new DirectoryInfo(Path.Combine(configuration.LocalStorageBasePath, configuration.DataProtectionContainer));
                if (!directory.Exists) directory.Create();
                buddyServices.Services.AddDataProtection().PersistKeysToFileSystem(directory);
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