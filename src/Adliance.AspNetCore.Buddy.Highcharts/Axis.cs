using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adliance.Highcharts.Buddy
{
    public class Axis
    {
        [JsonProperty("title")] public Title? Title { get; set; }
        [JsonProperty("labels")] public Label? Labels { get; set; }
        [JsonProperty("gridLineColor")] public string? GridLineColor { get; set; }
        [JsonProperty("gridLineDashStyle")] public string? GridLineDashStyle { get; set; }
        [JsonProperty("gridLineWidth")] public double? GridLineWidth { get; set; }
        [JsonProperty("tickWidth")] public double? TickWidth { get; set; }
        [JsonProperty("lineWidth")] public double? LineWidth { get; set; }
        [JsonProperty("endOnTick")] public bool? EndOnTick { get; set; }
        [JsonProperty("startOnTick")] public bool? StartOnTick { get; set; }
        [JsonProperty("tickInterval")] public double? TickInterval { get; set; }
        [JsonProperty("max")] public double? Max { get; set; }
        [JsonProperty("plotLines")] public IEnumerable<Plotline>? Plotlines { get; set; }
    }
}

