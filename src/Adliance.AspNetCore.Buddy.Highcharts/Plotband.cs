using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts
{
    public class Plotband
    {
        [JsonProperty("color")] public string? Color { get; set; }
        [JsonProperty("from")] public double? From { get; set; }
        [JsonProperty("to")] public double? To { get; set; }
        [JsonProperty("label")] public Label? Label { get; set; }
    }
}
