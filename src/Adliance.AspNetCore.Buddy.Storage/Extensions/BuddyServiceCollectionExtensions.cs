using System;
using System.IO;
using Adliance.AspNetCore.Buddy.Abstractions;
using Adliance.AspNetCore.Buddy.Storage.Azure;
using Adliance.AspNetCore.Buddy.Storage.Local;
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
            AddAzureStorage(buddyServices, configuration);
        }
        else if (configuration.UseLocalStorage)
        {
            AddLocalStorage(buddyServices, configuration);
        }
        else
        {
            throw new Exception("No storage configured.");
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

    public static IBuddyServiceCollection AddAzureStorage(
        this IBuddyServiceCollection buddyServices,
        IConfigurationSection configurationSection)
    {
        var configuration = configurationSection.Get<DefaultStorageConfiguration>();
        buddyServices.Services.Configure<DefaultStorageConfiguration>(configurationSection);
        ArgumentNullException.ThrowIfNull(configuration, "Storage Configuration");

        if (!configuration.UseAzureStorage) throw new Exception("Azure Storage configuration missing.");
        return AddAzureStorage(buddyServices, configuration);
    }

    public static IBuddyServiceCollection AddAzureStorage(
        this IBuddyServiceCollection buddyServices,
        IStorageConfiguration configuration)
    {
        if (configuration.UseAzureStorage)
        {
            buddyServices.Services.AddTransient<IStorage, AzureStorage>();
            buddyServices.Services.AddTransient<AzureStorage>();

            if (configuration is { ConfigureDataProtection: true }
                && !string.IsNullOrWhiteSpace(configuration.AzureStorageConnectionString)
                && !string.IsNullOrWhiteSpace(configuration.DataProtectionContainer))
            {
                buddyServices.Services.AddDataProtection().PersistKeysToAzureBlobStorage(configuration.AzureStorageConnectionString, configuration.DataProtectionContainer, "aspnetcore-keys");
            }
        }

        return buddyServices;
    }

    public static IBuddyServiceCollection AddLocalStorage(
        this IBuddyServiceCollection buddyServices,
        IConfigurationSection configurationSection)
    {
        var configuration = configurationSection.Get<DefaultStorageConfiguration>();
        buddyServices.Services.Configure<DefaultStorageConfiguration>(configurationSection);
        ArgumentNullException.ThrowIfNull(configuration, "Storage Configuration");

        if (!configuration.UseLocalStorage) throw new Exception("Local storage configuration missing.");
        return AddLocalStorage(buddyServices, configuration);
    }

    public static IBuddyServiceCollection AddLocalStorage(
        this IBuddyServiceCollection buddyServices,
        IStorageConfiguration configuration)
    {
        if (configuration.UseLocalStorage)
        {
            buddyServices.Services.AddTransient<IStorage, LocalStorage>();
            buddyServices.Services.AddTransient<LocalStorage>();

            if (configuration is { ConfigureDataProtection: true } && !string.IsNullOrWhiteSpace(configuration.DataProtectionContainer))
            {
                var directory = new DirectoryInfo(Path.Combine(configuration.LocalStorageBasePath ?? "", configuration.DataProtectionContainer));
                if (!directory.Exists) directory.Create();
                buddyServices.Services.AddDataProtection().PersistKeysToFileSystem(directory);
            }
        }

        return buddyServices;
    }
}
