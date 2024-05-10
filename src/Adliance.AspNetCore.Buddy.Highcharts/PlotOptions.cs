using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class PlotOptions
{
    [JsonProperty("series")] public SeriesPlotOptions? Series { get; set; }
    [JsonProperty("spline")] public SplinePlotOptions? Spline { get; set; }

    public class SeriesPlotOptions
    {
        [JsonProperty("animation")] public bool? Animation { get; set; }
        [JsonProperty("stickyTracking")] public bool? StickyTracking { get; set; }
        [JsonProperty("states")] public States? States { get; set; }
    }

    public class SplinePlotOptions
    {
        [JsonProperty("enableMouseTracking")] public bool? EnableMouseTracking { get; set; }
        [JsonProperty("marker")] public Marker? Marker { get; set; }
    }
}
