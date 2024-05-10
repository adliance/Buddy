using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class Theme
{
    [JsonProperty("fill")] public string? Fill { get; set; }
    [JsonProperty("style")] public Style? Style { get; set; }
    [JsonProperty("stroke")] public string? Stroke { get; set; }
    [JsonProperty("r")] public int? R { get; set; }
    [JsonProperty("states")] public States? States { get; set; }
}
