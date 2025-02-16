using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Adliance.AspNetCore.Buddy.Testing.InProcess;

public class InProcessResult<TEntryPoint> : IAsyncDisposable where TEntryPoint : class
{
    public WebApplicationFactory<TEntryPoint> Factory { get; set; } = null!;
    public HttpClient Client { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        Client?.Dispose();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Factory != null) await Factory.DisposeAsync().ConfigureAwait(false);
    }
}
