using System.Reflection;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry;

static class VersionHelper
{
    public static string GetAssemblyVersion()
    {
        var assembly = typeof(VersionHelper).GetTypeInfo().Assembly;
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "v1";
    }
}
