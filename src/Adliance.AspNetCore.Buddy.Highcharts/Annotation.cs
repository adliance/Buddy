using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adliance.Highcharts.Buddy
{
    public class Annotation
    {
        [JsonProperty("labels")] public IEnumerable<Label>? Labels { get; set; }
        [JsonProperty("draggable")] public string? Draggable { get; set; }
    }
}
