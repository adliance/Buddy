using System;
using System.IO;

namespace Adliance.AspNetCore.Buddy.Storage.Test
{
    public class MockedStorageConfiguration : IStorageConfiguration
    {
        public StorageType Type => StorageType.Local;
        public string LocalStorageBasePath => Path.GetTempPath();

        public string AzureStorageConnectionString
        {
            get
            {
                const string envName = "Adliance_Buddy_Tests__AzureStorageConnectionString";
                var envValue = Environment.GetEnvironmentVariable(envName);
                if (string.IsNullOrWhiteSpace(envValue)) throw new Exception($"No storage connection string found at env variable \"{envName}\".");
                return envValue;
            }
        }

        public bool AutomaticallyCreateDirectories => true;
        public bool ConfigureDataProtection => true;
        public string DataProtectionContainer => "dataprotection";
        public bool UseAzureStorage => false;
        public bool UseLocalStorage => false;
    }
}