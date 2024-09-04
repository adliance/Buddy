// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public interface IPdferConfiguration
{
    string ServerUrl { get; }
}

// ReSharper disable once UnusedType.Global
public class DefaultPdferConfiguration : IPdferConfiguration
{
    public string ServerUrl { get; set; } = "";
}
