using System;
using System.Reflection;
using OpenTelemetry.Exporter;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry;

/// <summary>
/// Specifies the configuration format.
/// </summary>
public interface IOpenTelemetryConfiguration
{
    /// <summary>
    /// Service name as reported to the OpenTelemetry collector.
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// Whether OpenTelemetry should also log to the console (default: false)
    /// </summary>
    public bool EnableConsoleExporter { get; set; }
    
    /// <summary>
    /// OpenTelemetry Protocol Exporter configuration.
    /// </summary>
    public OtlpExporterOptions OtlpExporter { get; set; }

    /// <summary>
    /// Configure OtlpExporter.
    /// </summary>
    /// <param name="options">The options to overwrite.</param>
    public void ConfigureExporterOptions(OtlpExporterOptions options)
    {
        options.Endpoint = OtlpExporter.Endpoint;
        options.Protocol = OtlpExporter.Protocol;
        options.Headers = OtlpExporter.Headers;
        options.TimeoutMilliseconds = OtlpExporter.TimeoutMilliseconds;
    }
}

/// <summary>
/// Default configuration for OpenTelemetry.
/// </summary>
public class DefaultOpenTelemetryConfiguration : IOpenTelemetryConfiguration
{
    /// <inheritdoc cref="IOpenTelemetryConfiguration.ServiceName"/>
    public string ServiceName { get; set; } = Assembly.GetEntryAssembly()?.GetName().Name ?? "adliance otel buddy";

    /// <inheritdoc cref="IOpenTelemetryConfiguration.EnableConsoleExporter"/>
    // ReSharper disable once RedundantDefaultMemberInitializer
    public bool EnableConsoleExporter { get; set; } = false;

    /// <inheritdoc cref="IOpenTelemetryConfiguration.OtlpExporter"/>
    public OtlpExporterOptions OtlpExporter { get; set; } = new()
    {
        Endpoint = new Uri("https://otc-grpc.adliance.dev"),
        TimeoutMilliseconds = 1000
    };
}