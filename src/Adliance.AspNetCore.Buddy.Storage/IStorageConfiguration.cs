namespace Adliance.AspNetCore.Buddy.Storage
{
    public interface IStorageConfiguration
    {
        StorageType Type { get; }
        string LocalStorageBasePath { get; }
        string AzureStorageConnectionString { get; }
        bool AutomaticallyCreateDirectories { get; }
        
        bool UseAzureStorage { get; }
        bool UseLocalStorage { get; }
    }

    public enum StorageType
    {
        Local,
        Azure
    }
}