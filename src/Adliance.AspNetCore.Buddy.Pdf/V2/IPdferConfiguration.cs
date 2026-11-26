using System.Reflection;

namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public interface IPdferConfiguration
{
    string ServerUrl { get; }
    string? ApiKeyPdf { get; }
    string? ApiKeyTemplate { get; }
    int TimeoutSeconds { get; }
    string? UserAgent { get; }
}

// ReSharper disable once UnusedType.Global
public class DefaultPdferConfiguration : IPdferConfiguration
{
    public string ServerUrl { get; set; } = "";
    public string? ApiKeyPdf { get; set; }
    public string? ApiKeyTemplate { get; set; }
    public int TimeoutSeconds { get; set; } = 60;
    public string? UserAgent { get; set; } = Assembly.GetEntryAssembly()?.GetName().Name + " (" + Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion + ")";
}
