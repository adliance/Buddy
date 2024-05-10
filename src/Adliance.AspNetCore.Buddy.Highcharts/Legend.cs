using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class Legend
{
    [JsonProperty("enabled")] public bool? Enabled { get; set; }
}
