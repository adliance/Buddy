using System;
using System.IO;

namespace Adliance.AspNetCore.Buddy.Storage.Test
{
    public class MockedStorageConfiguration : IStorageConfiguration
    {
        public StorageType Type => StorageType.Local;
        public string LocalStorageBasePath => Path.GetTempPath();

        public string AzureStorageConnectionString => Environment.GetEnvironmentVariable("Adliance_Buddy_Tests__AzureStorageConnectionString") ?? "";
        public bool AutomaticallyCreateDirectories => true;
        public bool ConfigureDataProtection => true;
        public string DataProtectionContainer => "dataprotection";
        public bool UseAzureStorage => false;
        public bool UseLocalStorage => false;
    }
}