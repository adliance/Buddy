using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry.Trace;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Middleware;

public class OpenTelemetryExceptionTracer : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        Activity.Current?.RecordException(exception);
        Activity.Current?.SetStatus(ActivityStatusCode.Error, "Unhandled exception");
        
        // Return false to continue with the default behavior
        return ValueTask.FromResult(false);
    }
}