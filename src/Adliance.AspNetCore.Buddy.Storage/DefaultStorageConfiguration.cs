using System;
using System.Diagnostics.CodeAnalysis;

namespace Adliance.AspNetCore.Buddy.Storage;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class DefaultStorageConfiguration : IStorageConfiguration
{
    /// <inheritdoc />
    public StorageType Type { get; set; }

    /// <inheritdoc />
    public string? LocalStorageBasePath { get; set; }

    /// <inheritdoc />
    public string? AzureStorageConnectionString { get; set; }

    /// <inheritdoc />
    public string? AzureStorageUrl { get; set; }

    public string? AzureStorageManagedIdentityClientId { get; set; }

    /// <inheritdoc />
    public bool AutomaticallyCreateDirectories { get; set; }

    public bool ConfigureDataProtection { get; set; }
    public string? DataProtectionContainer { get; set; } = "dataprotection";

    /// <inheritdoc />
    public bool UseAzureStorage
    {
        get
        {
            if (Type != StorageType.Azure) return false;

            if (string.IsNullOrWhiteSpace(AzureStorageConnectionString) && string.IsNullOrWhiteSpace(AzureStorageUrl))
            {
                throw new Exception("Azure Storage is configured, but connection information is missing.");
            }

            return true;
        }
    }

    /// <inheritdoc />
    public bool UseLocalStorage
    {
        get
        {
            if (Type != StorageType.Local) return false;
            if (string.IsNullOrWhiteSpace(LocalStorageBasePath))
            {
                throw new Exception("Local storage is configured, but base path is missing.");
            }

            return true;
        }
    }
}
