using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts
{
    public class Exporting
    {
        [JsonProperty("enabled")] public bool? Enabled { get; set; }
    }
}
