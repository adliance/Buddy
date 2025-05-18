namespace Adliance.AspNetCore.Buddy.Storage;

public interface IStorageConfiguration
{
    StorageType Type { get; }

    /// <summary>
    /// The local path where to save the storage files to.
    /// </summary>
    string LocalStorageBasePath { get; }

    /// <summary>
    /// The azure storage connection string.
    /// </summary>
    string AzureStorageConnectionString { get; }

    bool AutomaticallyCreateDirectories { get; }
    bool ConfigureDataProtection { get; }
    string DataProtectionContainer { get; }

    bool UseAzureStorage { get; }
    bool UseLocalStorage { get; }
}

public enum StorageType
{
    Local,
    Azure
}
