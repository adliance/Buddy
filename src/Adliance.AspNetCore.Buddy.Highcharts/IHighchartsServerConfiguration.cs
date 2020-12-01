// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace Adliance.AspNetCore.Buddy.Highcharts
{
    public interface IHighchartsServerConfiguration
    {
        string ServerUrl { get; }
    }

    // ReSharper disable once UnusedType.Global
    public class DefaultHighchartsServerConfiguration : IHighchartsServerConfiguration
    {
        public string ServerUrl { get; set; } = "";
    }
}