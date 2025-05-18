using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Adliance.AspNetCore.Buddy.OpenTelemetry.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

/// <summary>
///
/// </summary>
public static class BuddyServiceCollectionExtensions
{
    /// <summary>
    /// Adds auto-instrumentation for ASP.NET core, http client, EF core, and an OpenTelemetry exporter.
    /// </summary>
    /// <param name="buddyServices">The logging builder</param>
    /// <param name="configuration">The (parsed) configuration</param>
    /// <returns>The builder for further calls.</returns>
    public static IServiceCollection AddBuddyOpenTelemetry(
        this IServiceCollection buddyServices,
        IOpenTelemetryConfiguration configuration)
    {
        buddyServices.TryAddSingleton(configuration);

        IDictionary<string, object> resourceAttributes = new Dictionary<string, object>
        {
            { SemanticConventions.ResourceAttributeServiceVersion, configuration.ServiceVersion },
            { SemanticConventions.ResourceAttributeHostName, Dns.GetHostName() },
        };

        if (configuration.Environment != null)
            resourceAttributes.Add(SemanticConventions.ResourceAttributeDeploymentEnvironment,
                configuration.Environment);

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(configuration.ServiceName)
            .AddAttributes(resourceAttributes);

        var telServer = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddAspNetCoreInstrumentation(options => { options.RecordException = true; })
            .AddHttpClientInstrumentation(options => { options.RecordException = true; })
            .AddEntityFrameworkCoreInstrumentation()
            .AddOtlpExporter(configuration.ConfigureExporterOptions)
            .Build();

        buddyServices.AddSingleton(telServer);

        buddyServices.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(configuration.ConfigureExporterOptions)
            );

        buddyServices.AddSingleton(new HtmlHelper(configuration.ServiceName));

        buddyServices.AddExceptionHandler<OpenTelemetryExceptionTracer>();

        return buddyServices;
    }

    /// <summary>
    /// Adds auto-instrumentation for ASP.NET core, http client, EF core, and an OpenTelemetry exporter.
    /// </summary>
    /// <param name="buddyServices">The logging builder</param>
    /// <param name="openTelemetryConfigurationSection">The configuration section from appsettings.json</param>
    /// <returns>The builder for further calls.</returns>
    /// <exception cref="Exception">If the configuration section is faulty</exception>
    public static IServiceCollection AddBuddyOpenTelemetry(
        this IServiceCollection buddyServices,
        IConfigurationSection openTelemetryConfigurationSection)
    {
        var openTelemetryOptions = openTelemetryConfigurationSection.Get<DefaultOpenTelemetryConfiguration>() ??
                                   throw new Exception(
                                       $"Unable to load OpenTelemetry configuration from {openTelemetryConfigurationSection.Path}.");
        buddyServices.Configure<IOpenTelemetryConfiguration>(openTelemetryConfigurationSection);

        return AddBuddyOpenTelemetry(buddyServices, openTelemetryOptions);
    }

    /// <summary>
    /// Adds auto-instrumentation for Hangfire and an OpenTelemetry exporter.
    /// </summary>
    /// <param name="buddyServices">The logging builder</param>
    /// <param name="configuration">The (parsed) configuration</param>
    /// <returns>The builder for further calls.</returns>
    public static IServiceCollection AddBuddyOpenTelemetryHangfire(
        this IServiceCollection buddyServices,
        IOpenTelemetryConfiguration configuration)
    {
        buddyServices.TryAddSingleton(configuration);

        var telBackgroundJobs = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService($"{configuration.ServiceName} hangfire"))
            .AddHangfireInstrumentation(options => { options.RecordException = true; })
            .AddOtlpExporter(configuration.ConfigureExporterOptions)
            .Build();

        buddyServices.AddSingleton(telBackgroundJobs);

        return buddyServices;
    }

    /// <summary>
    /// Adds auto-instrumentation for Hangfire and an OpenTelemetry exporter.
    /// </summary>
    /// <param name="buddyServices">The logging builder</param>
    /// <param name="openTelemetryConfigurationSection">The configuration section from appsettings.json</param>
    /// <returns>The builder for further calls.</returns>
    /// <exception cref="Exception">If the configuration section is faulty</exception>
    public static IServiceCollection AddBuddyOpenTelemetryHangfire(
        this IServiceCollection buddyServices,
        IConfigurationSection openTelemetryConfigurationSection)
    {
        var openTelemetryOptions = openTelemetryConfigurationSection.Get<DefaultOpenTelemetryConfiguration>() ??
                                   throw new Exception(
                                       $"Unable to load OpenTelemetry configuration from {openTelemetryConfigurationSection.Path}.");
        if (buddyServices.All(d =>
                d.ServiceType != typeof(IConfigureOptions<IOpenTelemetryConfiguration>)))
            buddyServices.Configure<IOpenTelemetryConfiguration>(openTelemetryConfigurationSection);

        return AddBuddyOpenTelemetryHangfire(buddyServices, openTelemetryOptions);
    }
}
