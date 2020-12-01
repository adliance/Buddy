using Newtonsoft.Json;

namespace Adliance.Highcharts.Buddy
{
    public class Plotline
    {
        [JsonProperty("color")] public string? Color { get; set; }
        [JsonProperty("width")] public double? Width { get; set; }
        [JsonProperty("value")] public double? Value { get; set; }
    }
}
