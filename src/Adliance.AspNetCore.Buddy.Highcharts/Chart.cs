using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adliance.Highcharts.Buddy
{
    public class Chart
    {
        [JsonProperty("chart")] public Options? Options { get; set; }
        [JsonProperty("title")] public Title? Title { get; set; }
        [JsonProperty("credits")] public Credits? Credits { get; set; }
        [JsonProperty("exporting")] public Exporting? Exporting { get; set; }
        [JsonProperty("legend")] public Legend? Legend { get; set; }
        [JsonProperty("tooltip")] public Tooltip? Tooltip { get; set; }
        [JsonProperty("xAxis")] public Axis? AxisX { get; set; }
        [JsonProperty("yAxis")] public Axis? AxisY { get; set; }
        [JsonProperty("plotOptions")] public PlotOptions? PlotOptions { get; set; }
        [JsonProperty("series")] public IEnumerable<Series>? Series { get; set; }
        [JsonProperty("annotations")] public IEnumerable<Annotation>? Annotations { get; set; }
    }
}
