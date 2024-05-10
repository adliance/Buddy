using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class Position
{
    [JsonProperty("align")] public string? Align { get; set; }
    [JsonProperty("verticalAlign")] public string? VerticalAlign { get; set; }
    [JsonProperty("x")] public int? X { get; set; }
    [JsonProperty("y")] public int? Y { get; set; }
}
