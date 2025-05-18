using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

/// <summary>
///
/// </summary>
public static class BuddyLoggingExtensions
{
    /// <summary>
    /// Adds the OpenTelemetry exporter. Prints logs also to console.
    /// </summary>
    /// <param name="buddyLoggingBuilder">The logging builder</param>
    /// <param name="openTelemetryConfigurationSection">The configuration section from appsettings.json.</param>
    /// <returns>The builder for further calls.</returns>
    /// <exception cref="Exception">If the configuration section is faulty.</exception>
    public static ILoggingBuilder AddBuddyOpenTelemetry(
        this ILoggingBuilder buddyLoggingBuilder,
        IConfigurationSection openTelemetryConfigurationSection)
    {
        var openTelemetryOptions = openTelemetryConfigurationSection.Get<DefaultOpenTelemetryConfiguration>() ??
                                   throw new Exception(
                                       $"Unable to load OpenTelemetry configuration from {openTelemetryConfigurationSection.Path}.");

        return AddBuddyOpenTelemetry(buddyLoggingBuilder, openTelemetryOptions);
    }

    /// <summary>
    /// Adds the OpenTelemetry exporter. Prints logs also to console.
    /// </summary>
    /// <param name="buddyLoggingBuilder">The logging builder returned by AddBuddy.</param>
    /// <param name="configuration">The (parsed) configuration</param>
    /// <returns>The builder for further calls.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static ILoggingBuilder AddBuddyOpenTelemetry(
        this ILoggingBuilder buddyLoggingBuilder,
        IOpenTelemetryConfiguration configuration)
    {
        buddyLoggingBuilder.AddOpenTelemetry(options =>
        {
            options
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault().AddService(configuration.ServiceName))
                .AddOtlpExporter(configuration.ConfigureExporterOptions);

            if (configuration.EnableConsoleExporter)
            {
                options.AddConsoleExporter();
            }
        });
        return buddyLoggingBuilder;
    }
}
