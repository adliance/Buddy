using System.Collections.Generic;
using System.Linq;
using Adliance.Highcharts.Buddy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adliance.Highcharts.Buddy
{
    public class Tooltip
    {
        [JsonProperty("enabled")] public bool? Enabled { get; set; }
        [JsonProperty("crosshairs")] public IEnumerable<bool>? Crosshairs { get; set; }
        [JsonProperty("snap")] public int? Snap { get; set; }
    }
}
