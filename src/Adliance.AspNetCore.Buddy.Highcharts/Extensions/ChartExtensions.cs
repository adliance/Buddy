using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts.Extensions;

public static class ChartExtensions
{
    public static string ToJson(this Chart chart, Formatting formatting = Formatting.None)
    {
        return ToJsonInternal(chart, formatting);
    }

    public static string ToJson(this HighchartsServerParameter parameter, Formatting formatting = Formatting.None)
    {
        /*if (!string.IsNullOrWhiteSpace(parameter.Callback))
        {
            parameter.Callback = parameter.Callback.Replace("\r", "").Replace("\n", "");
            parameter.Callback = parameter.Callback.Trim();

            if (!parameter.Callback.StartsWith("function"))
            {
                parameter.Callback = $"function (chart) {{ {parameter.Callback} }}";
            }
        }*/

        return ToJsonInternal(parameter, formatting);
    }

    internal static string ToJsonInternal(object o, Formatting formatting = Formatting.None)
    {
        var json = JsonConvert.SerializeObject(o, formatting, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        return json;
    }
}
