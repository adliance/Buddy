# Adliance.AspNetCore.Buddy.OpenTelemetry

The OpenTelemetry Buddy makes instrumentation of ASP.NET Core web services easy.

## Features

- [Frontend instrumentation](#frontend-instrumentation)
- Backend instrumentation
- Background jobs with [Hangfire](https://www.hangfire.io/)

## Setup library in an ASP.NET project

The `IBuddyServiceCollection` of the `Adliance.AspNetCore.Buddy.Abstractions` package offers the `AddBuddy` method, which provides `AddOpenTelemetry` extensions to add the instrumentation and exporter.

```c#
using Adliance.AspNetCore.Buddy.Abstractions.Extensions;
using Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

// ...

public void ConfigureServices(IServiceCollection services)
{
  //...
  services.AddBuddy()
    .AddOpenTelemetry(Configuration.GetSection("OpenTelemetry"))    
  // if you use Hangfire, also add
  services.AddBuddy()
    .AddOpenTelemetryHangfire(Configuration.GetSection("OpenTelemetry"));
  }
}

public void Configure(IApplicationBuilder app)
{
    // if you want to instrument the frontend
    app.AddBuddy()
        .AddOpenTelemetryBrowserAssets();
}
```

To also send log messages from `Microsoft.Extensions.Logging`, `IBuddyLoggingBuilder` of the `Adliance.AspNetCore.Buddy.Abstractions` package offers another `AddBuddy` method,
which also provides an `AddOpenTelemetry` extension.

```c#
using Adliance.AspNetCore.Buddy.Abstractions.Extensions;
using Adliance.AspNetCore.Buddy.OpenTelemetry.Extensions;

// ...

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.ConfigureLogging((context, builder) =>
            {
                builder.AddBuddy()
                    .AddOpenTelemetry(context.Configuration.GetSection("OpenTelemetry"));
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
This can be achieved by adding a `meta` tag to the `head` section of the HTML.

```html
@using System.Diagnostics

@{ var traceParent = $"00-{Activity.Current?.TraceId}-{Activity.Current?.SpanId}-01"; }

<!--
  https://www.w3.org/TR/trace-context/
  Set the `traceparent` in the server's HTML template code. It should be
  dynamically generated server side to have the server's request trace Id,
  a parent span Id that was set on the server's request span, and the trace
  flags to indicate the server's sampling decision
  (01 = sampled, 00 = not sampled).
  '{version}-{traceId}-{spanId}-{sampleDecision}'
-->
<meta
  name="traceparent"
  content="@traceParent"
/>
```

The `AddOpenTelemetryBrowserAssets` extension method adds a middleware to serve the necessary JavaScript file at the
`/otel-js/telemetry.js` endpoint. This must also be referenced in your `_Layout.cshtml`:

```html
<script src="~/otel-js/telemetry.js?v=@version"></script>
<script>
    <!-- initialize the frontend instrumentation -->
    let telemetry = new Adliance.Buddy.Telemetry("example frontend");
    <!-- manually create a span -->
    telemetry.traceSpan("example span", () => "the result");
</script>
```