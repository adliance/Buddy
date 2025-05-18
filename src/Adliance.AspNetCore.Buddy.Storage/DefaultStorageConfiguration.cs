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
    public string LocalStorageBasePath { get; set; } = string.Empty;

    /// <inheritdoc />
    public string AzureStorageConnectionString { get; set; } = string.Empty;

    /// <inheritdoc />
    public bool AutomaticallyCreateDirectories { get; set; }
    public bool ConfigureDataProtection { get; set; }
    public string DataProtectionContainer { get; set; } = "dataprotection";

    /// <inheritdoc />
    public bool UseAzureStorage
    {
        get
        {
            if (Type == StorageType.Azure)
            {
                if (string.IsNullOrWhiteSpace(AzureStorageConnectionString))
                {
                    throw new Exception("Azure Storage is configured, but connection string is missing.");
                }

                return true;
            }

            return false;
        }
    }

    /// <inheritdoc />
    public bool UseLocalStorage
    {
        get
        {
            if (Type == StorageType.Local)
            {
                if (string.IsNullOrWhiteSpace(LocalStorageBasePath))
                {
                    throw new Exception("Local storage is configured, but base path is missing.");
                }

                return true;
            }

            return false;
        }
    }
}
