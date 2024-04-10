using System.Diagnostics;
using System.Globalization;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry;

public class HtmlHelper(string serviceName)
{
    private readonly string _scriptUrl =
        string.Format(CultureInfo.InvariantCulture, "/otel-{0}/telemetry.js", VersionHelper.GetAssemblyVersion());

    private string IncludeScriptTag =>
        string.Format(CultureInfo.InvariantCulture, Resources.JavaScriptInclude, _scriptUrl);

    private readonly string _initializationSnippet =
        string.Format(CultureInfo.InvariantCulture, Resources.JavaScriptInitialization, serviceName);

    /// <summary>
    /// Includes the telemetry SDK and initializes it with the configured service name + " web"
    /// </summary>
    public string IncludeAndInitializeJavascript => string.Concat(IncludeScriptTag, _initializationSnippet);

    // private readonly string _traceParent = $"00-{Activity.Current?.TraceId}-{Activity.Current?.SpanId}-01";

    /// <summary>
    /// Generates a traceparent meta tag to have the server's request trace Id,
    /// a parent span Id that was set on the server's request span, and the trace
    /// flags to indicate the server's sampling decision
    ///     (01 = sampled, 00 = not sampled).
    /// '{version}-{traceId}-{spanId}-{sampleDecision}'
    ///
    /// https://www.w3.org/TR/trace-context/
    /// </summary>
    public string TraceParentTag
    {
        get
        {
            var traceParent = $"00-{Activity.Current?.TraceId}-{Activity.Current?.SpanId}-01";
            return string.Format(CultureInfo.InvariantCulture, Resources.TraceParentTag, traceParent);
        }
    }
}