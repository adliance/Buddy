using System;
using OpenTelemetry.Exporter;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry;

public interface IOpenTelemetryConfiguration
{
    public string ServiceName { get; set; }

    public OtlpExporterOptions OtlpExporter { get; set; }

    public void ConfigureExporterOptions(OtlpExporterOptions options)
    {
        options.Endpoint = OtlpExporter.Endpoint;
        options.Protocol = OtlpExporter.Protocol;
        options.Headers = OtlpExporter.Headers;
        options.TimeoutMilliseconds = OtlpExporter.TimeoutMilliseconds;
    }
}

public class DefaultOpenTelemetryConfiguration : IOpenTelemetryConfiguration
{
    public string ServiceName { get; set; } = "adliance otel buddy";

    public OtlpExporterOptions OtlpExporter { get; set; } = new()
    {
        Endpoint = new Uri("https://otc-grpc.adliance.dev"),
        TimeoutMilliseconds = 1000
    };
}