using System;
using OpenTelemetry.Exporter;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry;

public interface IOpenTelemetryConfiguration
{
    public string ServiceName { get; set; }

    public OtlpExporterOptions OtlpExporterOptions { get; set; }
}

public class DefaultOpenTelemetryConfiguration : IOpenTelemetryConfiguration
{
    public string ServiceName { get; set; } = "adliance otel buddy";

    public OtlpExporterOptions OtlpExporterOptions { get; set; } = new()
    {
        Endpoint = new Uri("localhost:4317")
    };
}