using System;
using System.Linq;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter;
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
        private const string OtlpExporterConfigurationName = "buddy-otlp-exporter";

        public static IBuddyServiceCollection AddOpenTelemetry(
            this IBuddyServiceCollection buddyServices,
            IOpenTelemetryConfiguration configuration)
        {
            buddyServices.Services.TryAddSingleton(configuration);

            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(configuration.ServiceName);
            
            var telServer = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(OtlpExporterConfigurationName, options => {})
                .Build();

            buddyServices.Services.AddSingleton(telServer);
            
            buddyServices.Services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(OtlpExporterConfigurationName, options => {})
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
            
            // set our defaults
            buddyServices.Services.Configure<OtlpExporterOptions>(OtlpExporterConfigurationName, options =>
            {
                options.Endpoint = openTelemetryOptions.OtlpExporterOptions.Endpoint;
                options.Headers = openTelemetryOptions.OtlpExporterOptions.Headers;
                options.Protocol = openTelemetryOptions.OtlpExporterOptions.Protocol;
            });
            // overwrite with appsettings
            buddyServices.Services.Configure<OtlpExporterOptions>(OtlpExporterConfigurationName, openTelemetryConfigurationSection.GetSection("otlp"));

            return AddOpenTelemetry(buddyServices, openTelemetryOptions);
        }
        
        public static IBuddyServiceCollection AddOpenTelemetryHangfire(
            this IBuddyServiceCollection buddyServices,
            IOpenTelemetryConfiguration configuration)
        {
            buddyServices.Services.TryAddSingleton(configuration);
            
            var telBackgroundJobs = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService($"{configuration.ServiceName} hangfire"))
                .AddHangfireInstrumentation()
                .AddOtlpExporter(OtlpExporterConfigurationName, options => {})
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
                                           $"Unable to load email configuration from {openTelemetryConfigurationSection.Path}.");
            if (buddyServices.Services.All(d => d.ServiceType != typeof(IConfigureOptions<IOpenTelemetryConfiguration>)))
                buddyServices.Services.Configure<IOpenTelemetryConfiguration>(openTelemetryConfigurationSection);
            
            if (buddyServices.Services.All(d => d.ServiceType != typeof(IConfigureOptions<OtlpExporterOptions>)))
                buddyServices.Services.Configure<OtlpExporterOptions>(OtlpExporterConfigurationName, openTelemetryConfigurationSection.GetSection("otlp"));

            return AddOpenTelemetryHangfire(buddyServices, openTelemetryOptions);
        }
    }
}