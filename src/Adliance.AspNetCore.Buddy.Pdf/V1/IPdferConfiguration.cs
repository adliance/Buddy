// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

using System.Reflection;

namespace Adliance.AspNetCore.Buddy.Pdf.V1;

public interface IPdferConfiguration
{
    string ServerUrl { get; }
    string? UserAgent { get; }
    int TimeoutSeconds { get; }
}

// ReSharper disable once UnusedType.Global
public class DefaultPdferConfiguration : IPdferConfiguration
{
    public string ServerUrl { get; set; } = "";
    public int TimeoutSeconds { get; set; } = 60;
    public string? UserAgent { get; set; } = Assembly.GetEntryAssembly()?.FullName
                                             ?? Assembly.GetExecutingAssembly().FullName
                                             ?? Assembly.GetCallingAssembly().FullName;
}
