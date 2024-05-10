using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class Label
{
    [JsonProperty("point")] public LabelPoint? Point { get; set; }
    [JsonProperty("style")] public Style? Style { get; set; }
    [JsonProperty("text")] public string? Text { get; set; }
    [JsonProperty("borderWidth")] public double? BorderWidth { get; set; }
    [JsonProperty("borderColor")] public string? BorderColor { get; set; }
    [JsonProperty("backgroundColor")] public string? BackgroundColor { get; set; }
    [JsonProperty("allowOverlap")] public bool? AllowOverlap { get; set; }

    /// <summary>
    /// The alignment of the annotation's label. If right, the right side of the label should be touching the point.
    /// Allowed values: center | left | right.
    /// Defaults to center.
    /// </summary>
    [JsonProperty("align")] public string? Align { get; set; }

    public class LabelPoint
    {
        public LabelPoint() { }

        public LabelPoint(double? x, double? y)
        {
            X = x;
            Y = y;
        }

        [JsonProperty("x")] public double? X { get; set; }
        [JsonProperty("y")] public double? Y { get; set; }
    }
}
