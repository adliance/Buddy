using System;
using System.Reflection;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.GuiKit.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationInsightsIfConfigured(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";
            Console.WriteLine($"Using Application Insights with app version \"{version}\" and connection string: \"{connectionString}\".");
            services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
            {
                ConnectionString = connectionString,
                ApplicationVersion = version
            });

            services.AddTransient<AppInsightsJavaScript>();
        }
        else
        {
            Console.WriteLine("No Application Insights connection string configured.");
        }
    }
}
