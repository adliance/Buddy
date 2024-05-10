using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class Style
{
    [JsonProperty("fontSize")] public string? FontSize { get; set; }
    [JsonProperty("fontFamily")] public string? FontFamily { get; set; }
    [JsonProperty("color")] public string? Color { get; set; }
    [JsonProperty("cursor")] public string? Cursor { get; set; }
}
