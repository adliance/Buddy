using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Containers;

public class ContainerResult : IAsyncDisposable
{
    public IImage Image { get; internal set; } = null!;
    public IContainer Container { get; internal set; } = null!;
    public Uri Url { get; internal set; } = null!;
    public HttpClient Client { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        Client?.Dispose();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Container != null) await Container.DisposeAsync().ConfigureAwait(false);
    }
}
