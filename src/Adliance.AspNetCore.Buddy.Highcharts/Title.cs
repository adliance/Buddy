using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class Title
{
    [JsonProperty("text")] public string? Text { get; set; }
    [JsonProperty("style")] public Style? Style { get; set; }
    [JsonProperty("y")] public int? Y { get; set; }
}
