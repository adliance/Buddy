using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Adliance.AspNetCore.Buddy.Storage.Test;

public class MockedStorageConfiguration : IStorageConfiguration
{
    public StorageType Type => StorageType.Local;
    public string LocalStorageBasePath => Path.GetTempPath();

    public string AzureStorageConnectionString => GetEnvironmentVariable("Adliance_Buddy_Tests__AzureStorageConnectionString");

    public bool AutomaticallyCreateDirectories => true;
    public bool ConfigureDataProtection => true;
    public string DataProtectionContainer => "dataprotection";
    public bool UseAzureStorage => false;
    public bool UseLocalStorage => false;

    public string GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name)
               ?? Environment.GetEnvironmentVariable(name.ToUpper())
               ?? Environment.GetEnvironmentVariable(name.ToLower())
               ?? throw BuildEnvironmentVariableException(name);
    }

    private static Exception BuildEnvironmentVariableException(string name)
    {
        var sb = new StringBuilder();
        sb.AppendLine(CultureInfo.InvariantCulture, $"Environment variable \"{name}\" missing. Available environment variables are:");
        foreach (var o in Environment.GetEnvironmentVariables()) sb.AppendLine(o.ToString());
        throw new Exception(sb.ToString());
    }
}
