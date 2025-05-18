using System;
using System.IO;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Adliance.AspNetCore.Buddy.Storage.Extensions;

/// <summary>
/// Extension methods for <see cref="IBuddyServiceCollection"/>.
/// </summary>
public static class BuddyServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure or local storage services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="buddyServices">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The <see cref="IStorageConfiguration"/> instance.</param>
    /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
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
            buddyServices.Services
                .AddDataProtection()
                .PersistKeysToAzureBlobStorage(configuration.AzureStorageConnectionString, configuration.DataProtectionContainer, "aspnetcore-keys");
        }
        else if (configuration.ConfigureDataProtection && configuration.UseLocalStorage && !string.IsNullOrWhiteSpace(configuration.DataProtectionContainer))
        {
            var directory = new DirectoryInfo(Path.Combine(configuration.LocalStorageBasePath, configuration.DataProtectionContainer));
            if (!directory.Exists) directory.Create();
            buddyServices.Services
                .AddDataProtection()
                .PersistKeysToFileSystem(directory);
        }

        return buddyServices;
    }

    /// <summary>
    /// Adds Azure or local storage to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="buddyServices">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
    /// <param name="configurationSection">The <see cref="IConfigurationSection"/> configuration section.</param>
    /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
    public static IBuddyServiceCollection AddStorage(
        this IBuddyServiceCollection buddyServices,
        IConfigurationSection configurationSection)
    {
        var configuration = configurationSection.Get<DefaultStorageConfiguration>();
        buddyServices.Services.Configure<DefaultStorageConfiguration>(configurationSection);
        ArgumentNullException.ThrowIfNull(configuration, "Storage Configuration");
        return AddStorage(buddyServices, configuration);
    }
}
