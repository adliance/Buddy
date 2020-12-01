using Newtonsoft.Json;

namespace Adliance.Highcharts.Buddy
{
    public class Marker
    {
        [JsonProperty("enabled")] public bool? Enabled { get; set; }
        [JsonProperty("symbol")] public string? Symbol { get; set; }
        [JsonProperty("radius")] public double? Radius { get; set; }
        [JsonProperty("lineWidth")] public double? LineWidth { get; set; }
        [JsonProperty("lineColor")] public string? LineColor { get; set; }
        [JsonProperty("color")] public string? Color { get; set; }
        [JsonProperty("enabledThreshold")] public double? EnabledTreshold { get; set; }
    }
}
