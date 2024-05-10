using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts;

public class HighchartsServerParameter
{
    [JsonProperty("infile")] public Chart? Chart { get; set; } = new Chart();
    [JsonProperty("type")] public string? Format { get; set; } = "png";
    [JsonProperty("width")] public int? Width { get; set; }
    [JsonProperty("scale")] public double? Scale { get; set; }
    [JsonProperty("callback")] public string? Callback { get; set; }
    [JsonProperty("resources")] public ResourcesParameter Resources { get; set; } = new ResourcesParameter();

    public class ResourcesParameter
    {
        [JsonProperty("files")] public string? Files { get; set; }
        [JsonProperty("css")] public string? Css { get; set; }
        [JsonProperty("Js")] public string? JavaScript { get; set; }
    }
}
