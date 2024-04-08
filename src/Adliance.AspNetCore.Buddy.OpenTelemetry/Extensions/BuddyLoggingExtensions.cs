using System;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

public static class BuddyLoggingExtensions
{
    /// <summary>
    /// Adds the OpenTelemetry exporter. Prints logs also to console.
    /// </summary>
    /// <param name="buddyLoggingBuilder">The logging builder returned by <see cref="Adliance.AspNetCore.Buddy.Abstractions.Extensions.LoggingBuilderExtensions.AddBuddy">AddBuddy</see>.</param>
    /// <param name="openTelemetryConfigurationSection">The configuration section from appsettings.json.</param>
    /// <returns>The builder for further calls.</returns>
    /// <exception cref="Exception">If the configuration section is faulty.</exception>
    public static IBuddyLoggingBuilder AddOpenTelemetry(
        this IBuddyLoggingBuilder buddyLoggingBuilder,
        IConfigurationSection openTelemetryConfigurationSection)
    {
        var openTelemetryOptions = openTelemetryConfigurationSection.Get<DefaultOpenTelemetryConfiguration>() ??
                                   throw new Exception(
                                       $"Unable to OpenTelemetry email configuration from {openTelemetryConfigurationSection.Path}.");

        return AddOpenTelemetry(buddyLoggingBuilder, openTelemetryOptions);
    }

    /// <summary>
    /// Adds the OpenTelemetry exporter. Prints logs also to console.
    /// </summary>
    /// <param name="buddyLoggingBuilder">The logging builder returned by <see cref="Adliance.AspNetCore.Buddy.Abstractions.Extensions.LoggingBuilderExtensions.AddBuddy">AddBuddy</see>.</param>
    /// <param name="configuration">The (parsed) configuration</param>
    /// <returns>The builder for further calls.</returns>
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
                .AddOtlpExporter(configuration.ConfigureExporterOptions);
        });
        return buddyLoggingBuilder;
    }
}