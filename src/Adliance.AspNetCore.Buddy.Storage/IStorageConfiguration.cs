namespace Adliance.AspNetCore.Buddy.Storage;

public interface IStorageConfiguration
{
    StorageType Type { get; }

    /// <summary>
    /// The local path where to save the storage files to.
    /// </summary>
    string? LocalStorageBasePath { get; }

    /// <summary>
    /// The azure storage connection string, if you want to use authentication via access keys.
    /// </summary>
    string? AzureStorageConnectionString { get; }

    /// <summary>
    /// The azure storage server URL (eg. "https://your-storage-account.blob.core.windows.net"), if you want to use authentication using managed identities.
    /// </summary>
    string? AzureStorageUrl { get; }

    /// <summary>
    /// Specify the client id of the managed identity to use.
    /// Leave empty if you want to use system-managed identities.
    /// </summary>
    string? AzureStorageManagedIdentityClientId { get; }

    bool AutomaticallyCreateDirectories { get; }
    bool ConfigureDataProtection { get; }
    bool IgnoreCertificateErrors { get; }
    string? DataProtectionContainer { get; }

    bool UseAzureStorage { get; }
    bool UseLocalStorage { get; }
}

public enum StorageType
{
    Local,
    Azure
}
