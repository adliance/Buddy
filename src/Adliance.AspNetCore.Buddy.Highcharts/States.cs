using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts
{
    public class States
    {
        [JsonProperty("hover")] public State? Hover { get; set; }
        [JsonProperty("inactive")] public State? Inactive { get; set; }

        public class State
        {
            [JsonProperty("fill")] public string? Fill { get; set; }
            [JsonProperty("style")] public Style? Style { get; set; }
            [JsonProperty("stroke")] public string? Stroke { get; set; }
            [JsonProperty("opacity")] public double? Opacity { get; set; }
        }
    }
}
