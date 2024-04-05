using System;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

public static class BuddyLoggingExtensions
{
    public static IBuddyLoggingBuilder AddOpenTelemetry(
        this IBuddyLoggingBuilder buddyLoggingBuilder,
        IConfigurationSection openTelemetryConfigurationSection)
    {
        var openTelemetryOptions = openTelemetryConfigurationSection.Get<DefaultOpenTelemetryConfiguration>() ??
                                   throw new Exception(
                                       $"Unable to load email configuration from {openTelemetryConfigurationSection.Path}.");

        return AddOpenTelemetry(buddyLoggingBuilder, openTelemetryOptions);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static IBuddyLoggingBuilder AddOpenTelemetry(
        this IBuddyLoggingBuilder buddyLoggingBuilder,
        IOpenTelemetryConfiguration configuration)
    {
        buddyLoggingBuilder.Builder.AddOpenTelemetry(options =>
        {
            options
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault().AddService(configuration.ServiceName))
                .AddConsoleExporter()
                .AddOtlpExporter(exporterOptions =>
                {
                    if (configuration.OtlpExporterOptions?.Endpoint != null)
                        exporterOptions.Endpoint = configuration.OtlpExporterOptions.Endpoint;
                    if (configuration.OtlpExporterOptions?.Headers != null)
                        exporterOptions.Headers = configuration.OtlpExporterOptions.Headers;
                });
        });
        return buddyLoggingBuilder;
    }
}