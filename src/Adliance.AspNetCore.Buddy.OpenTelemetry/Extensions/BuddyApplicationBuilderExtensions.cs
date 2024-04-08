using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

public static class BuddyApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a static file server for /otel-js/telemetry.js.
    /// </summary>
    /// <param name="applicationBuilder">The application builder.</param>
    /// <returns>The application builder to allow further calls.</returns>
    public static IApplicationBuilder AddBuddyOpenTelemetryBrowserAssets(
        this IApplicationBuilder applicationBuilder)
    {
        var assembly = typeof(BuddyApplicationBuilderExtensions).GetTypeInfo().Assembly;
        var embeddedFileProvider = new EmbeddedFileProvider(
            assembly,
            "Adliance.AspNetCore.Buddy.OpenTelemetry.wwwroot.lib.dist"
        );
        
        applicationBuilder.UseStaticFiles(new StaticFileOptions
        {
            
            FileProvider = embeddedFileProvider,
            RequestPath = new PathString("/otel-js")
        });

        return applicationBuilder;
    }
}