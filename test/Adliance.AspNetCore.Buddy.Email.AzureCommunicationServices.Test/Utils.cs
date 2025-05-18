using System.Globalization;
using System.Text;

namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices.Test;

public static class Utils
{
    public static string GetEnvironmentVariable(string name)
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
