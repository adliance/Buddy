using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry.Trace;

namespace Adliance.AspNetCore.Buddy.OpenTelemetry.Middleware;

/// <summary>
/// This exception handler adds the unhandled exception to the current trace.
/// </summary>
public class OpenTelemetryExceptionTracer : IExceptionHandler
{
    /// <summary>
    /// Adds exception to current trace.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>False, so the exception will be handled further</returns>
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        Activity.Current?.RecordException(exception);
#pragma warning restore CS0618 // Type or member is obsolete
        Activity.Current?.SetStatus(ActivityStatusCode.Error, "Unhandled exception");
        
        // Return false to continue with the default behavior
        return ValueTask.FromResult(false);
    }
}