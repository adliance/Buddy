namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public interface IPdferConfiguration
{
    string ServerUrl { get; }
    string? ApiKeyPdf { get; }
    string? ApiKeyTemplate { get; }
}

// ReSharper disable once UnusedType.Global
public class DefaultPdferConfiguration : IPdferConfiguration
{
    public string ServerUrl { get; set; } = "";
    public string? ApiKeyPdf { get; set; }
    public string? ApiKeyTemplate { get; set; }
}
