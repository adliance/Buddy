# Adliance.AspNetCore.Buddy.OpenTelemetry

The OpenTelemetry Buddy makes instrumentation of ASP.NET Core web services easy.

## Features

- [Frontend instrumentation](#frontend-instrumentation)
- Backend instrumentation
- Background jobs with [Hangfire](https://www.hangfire.io/)

## Setup library in an ASP.NET project

The package offers the `AddBuddyOpenTelemetry` extensions to add the instrumentation and exporter.

```c#
using Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

// ...

public void ConfigureServices(IServiceCollection services)
{
  //...
  services.AddBuddyOpenTelemetry(Configuration.GetSection("OpenTelemetry"))    
  // if you use Hangfire, also add
  services.AddBuddyOpenTelemetryHangfire(Configuration.GetSection("OpenTelemetry"));
}

public void Configure(IApplicationBuilder app)
{
    // if you want to instrument the frontend
    app.UseBuddyOpenTelemetryBrowserAssets();
}
```

To also send log messages from `Microsoft.Extensions.Logging`, the package offers another `AddBuddyOpenTelemetry` extension.

```c#
using Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

// ...

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.ConfigureLogging((context, builder) =>
            {
                builder.AddBuddyOpenTelemetry(context.Configuration.GetSection("OpenTelemetry"));
            });
        });
```

### Configuration (appsettings.json)

Add a section in the configuration of your project and add at least the following configuration:

```json
{
  "OpenTelemetry": {
    "ServiceName": "example server"
  }
}
```

It is also possible to overwrite defaults for the exporter like this:

```json
{
  "OpenTelemetry": {
    "ServiceName": "example server",
    "OtlpExporter": {
      "Endpoint": "http://localhost:4317",
      "Protocol": "0", // 0 ... gRPC, 1 ... http/protobuf
      "Headers": "X-Api-Key=123,X-Api-Version=1.0",
      "TimeoutMilliseconds": 60000
    }
  }
}
```

### Frontend instrumentation

It is important to pass a trace parent to the frontend tracing SDK, such that the spans can be grouped together later on.
This can be achieved by adding a `meta` tag to the `head` section of the HTML by

1. Adding the following code to `_ViewImports.cshtml`:
```cshtml
@inject Adliance.AspNetCore.Buddy.OpenTelemetry.HtmlHelper OtelHtmlHelper
```

2. Adding the following in the `head` section of your `_Layout.cshtml` before closing the `body` section:

```html
@Html.Raw(OtelHtmlHelper.TraceParentTag)
```

The `UseBuddyOpenTelemetryBrowserAssets` extension method adds a middleware to serve the necessary JavaScript file at the
`/otel-$version/telemetry.js` endpoint. This must also be referenced in your `_Layout.cshtml`:

```html
@Html.Raw(OtelHtmlHelper.IncludeAndInitializeJavascript)

<!-- optionally, you also have the option for manual instrumentation -->
<script>
    <!-- manually create a span -->
    window.buddyTelemetry.traceSpan("example span", () => "the result");
</script>
```