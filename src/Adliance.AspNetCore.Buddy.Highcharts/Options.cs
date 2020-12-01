using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts
{
    public class Options
    {
        [JsonProperty("type")] public string? Type { get; set; }
        [JsonProperty("animation")] public bool? Animation { get; set; }
        [JsonProperty("zoomType")] public string? ZoomType { get; set; }
        [JsonProperty("width")] public int? Width { get; set; }
        [JsonProperty("height")] public int? Height { get; set; }
        [JsonProperty("spacingRight")] public int? SpacingRight { get; set; }
        [JsonProperty("spacingTop")] public int? SpacingTop { get; set; }
        [JsonProperty("spacingLeft")] public int? SpacingLeft { get; set; }
        [JsonProperty("spacingBottom")] public int? SpacingBottom { get; set; }
        [JsonProperty("style")] public Style? Style { get; set; }
        [JsonProperty("resetZoomButton")] public ResetZoomButtonSettings? ResetZoomButton { get; set; }

        public class ResetZoomButtonSettings
        {
            [JsonProperty("theme")] public Theme? Theme { get; set; }
        }
    }
}
