using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adliance.Highcharts.Buddy
{
    public class Series
    {
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("color")] public string? Color { get; set; }
        [JsonProperty("type")] public string? Type { get; set; }
        [JsonProperty("dashStyle")] public string? DashStyle { get; set; }
        [JsonProperty("lineWidth")] public double? LineWidth { get; set; }
        [JsonProperty("marker")] public Marker? Marker { get; set; }
        [JsonProperty("turboThreshold")] public int? TurboThreshold { get; set; }

        [JsonProperty("data")] public IEnumerable<DataPoint>? Data { get; set; }
    }

    public class DataPoint
    {
        public DataPoint() { }

        public DataPoint(double x, double y, string? name = null, Marker? marker = null)
        {
            X = x;
            Y = y;
            Name = name;
            Marker = marker;
        }

        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("x")] public double? X { get; set; }
        [JsonProperty("y")] public double? Y { get; set; }
        [JsonProperty("marker")] public Marker? Marker { get; set; }
    }
}
