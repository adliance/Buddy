using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class Tooltip
{
    [JsonProperty("enabled")] public bool? Enabled { get; set; }
    [JsonProperty("crosshairs")] public IEnumerable<bool>? Crosshairs { get; set; }
    [JsonProperty("snap")] public int? Snap { get; set; }
}
