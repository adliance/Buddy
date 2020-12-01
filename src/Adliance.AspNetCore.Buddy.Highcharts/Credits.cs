using Newtonsoft.Json;

namespace Adliance.Highcharts.Buddy
{
    public class Credits
    {
        [JsonProperty("enabled")] public bool? Enabled { get; set; }
        [JsonProperty("text")] public string? Text { get; set; }
        [JsonProperty("href")] public string? Href { get; set; }   
        [JsonProperty("style")] public Style? Style { get; set; }
        [JsonProperty("position")] public Position? Position { get; set; }
    }
}
