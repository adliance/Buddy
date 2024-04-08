using System;
using System.Linq;
using Adliance.AspNetCore.Buddy.Abstractions;
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

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions
{
    public static class BuddyServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddOpenTelemetry(
            this IBuddyServiceCollection buddyServices,
            IOpenTelemetryConfiguration configuration)
        {
            buddyServices.Services.TryAddSingleton(configuration);

            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(configuration.ServiceName);

            var telServer = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation(options => { options.RecordException = true; })
                .AddHttpClientInstrumentation(options => { options.RecordException = true; })
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(configuration.ConfigureExporterOptions)
                .Build();

            buddyServices.Services.AddSingleton(telServer);

            buddyServices.Services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(configuration.ConfigureExporterOptions)
                );
            
            return buddyServices;
        }

        public static IBuddyServiceCollection AddOpenTelemetry(
            this IBuddyServiceCollection buddyServices,
            IConfigurationSection openTelemetryConfigurationSection)
        {
            var openTelemetryOptions = openTelemetryConfigurationSection.Get<DefaultOpenTelemetryConfiguration>() ??
                                       throw new Exception(
                                           $"Unable to load OpenTelemetry configuration from {openTelemetryConfigurationSection.Path}.");
            buddyServices.Services.Configure<IOpenTelemetryConfiguration>(openTelemetryConfigurationSection);

            return AddOpenTelemetry(buddyServices, openTelemetryOptions);
        }

        public static IBuddyServiceCollection AddOpenTelemetryHangfire(
            this IBuddyServiceCollection buddyServices,
            IOpenTelemetryConfiguration configuration)
        {
            buddyServices.Services.TryAddSingleton(configuration);

            var telBackgroundJobs = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService($"{configuration.ServiceName} hangfire"))
                .AddHangfireInstrumentation(options => { options.RecordException = true; })
                .AddOtlpExporter(configuration.ConfigureExporterOptions)
                .Build();

            buddyServices.Services.AddSingleton(telBackgroundJobs);

            return buddyServices;
        }

        public static IBuddyServiceCollection AddOpenTelemetryHangfire(
            this IBuddyServiceCollection buddyServices,
            IConfigurationSection openTelemetryConfigurationSection)
        {
            var openTelemetryOptions = openTelemetryConfigurationSection.Get<DefaultOpenTelemetryConfiguration>() ??
                                       throw new Exception(
                                           $"Unable to load OpenTelemetry configuration from {openTelemetryConfigurationSection.Path}.");
            if (buddyServices.Services.All(d =>
                    d.ServiceType != typeof(IConfigureOptions<IOpenTelemetryConfiguration>)))
                buddyServices.Services.Configure<IOpenTelemetryConfiguration>(openTelemetryConfigurationSection);

            return AddOpenTelemetryHangfire(buddyServices, openTelemetryOptions);
        }
    }
}