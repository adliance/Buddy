// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Adliance.AspNetCore.Buddy.Pdf
{
    public interface IPdferConfiguration
    {
        string ServerUrl { get; }
    }

    // ReSharper disable once UnusedType.Global
    public class DefaultPdferConfiguration : IPdferConfiguration
    {
        public string ServerUrl { get; set; } = "";
    }
}