using System;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

/// <summary>
///
/// </summary>
public static class BuddyApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a static file server for /otel-js/telemetry.js.
    /// </summary>
    /// <param name="applicationBuilder">The application builder.</param>
    /// <returns>The application builder to allow further calls.</returns>
    public static IApplicationBuilder UseBuddyOpenTelemetryBrowserAssets(
        this IApplicationBuilder applicationBuilder)
    {
        var assembly = typeof(BuddyApplicationBuilderExtensions).GetTypeInfo().Assembly;
        var embeddedFileProvider = new EmbeddedFileProvider(
            assembly,
            "Adliance.AspNetCore.Buddy.OpenTelemetry.wwwroot.lib.dist"
        );

        var version = VersionHelper.GetAssemblyVersion();

        applicationBuilder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = embeddedFileProvider,
            RequestPath = new PathString($"/otel-{version}"),
            OnPrepareResponse = ctx =>
            {
                // Cache static file for 365 days
                ctx.Context.Response.Headers.CacheControl = new StringValues("public,max-age=31536000, immutable");
                ctx.Context.Response.Headers.Expires =
                    new StringValues(DateTime.UtcNow.AddDays(365).ToString("R", CultureInfo.InvariantCulture));
            }
        });

        return applicationBuilder;
    }
}
