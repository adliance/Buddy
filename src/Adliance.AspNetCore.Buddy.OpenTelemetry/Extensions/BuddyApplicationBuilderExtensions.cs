using System.Reflection;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

public static class BuddyApplicationBuilderExtensions
{
    public static IBuddyApplicationBuilder AddOpenTelemetryBrowserAssets(
        this IBuddyApplicationBuilder applicationBuilder)
    {
        var assembly = typeof(BuddyApplicationBuilderExtensions).GetTypeInfo().Assembly;
        var embeddedFileProvider = new EmbeddedFileProvider(
            assembly,
            "Adliance.AspNetCore.Buddy.OpenTelemetry.wwwroot.lib.dist"
        );
        
        applicationBuilder.Builder.UseStaticFiles(new StaticFileOptions
        {
            
            FileProvider = embeddedFileProvider,
            RequestPath = new PathString("/otel-js")
        });

        return applicationBuilder;
    }
}