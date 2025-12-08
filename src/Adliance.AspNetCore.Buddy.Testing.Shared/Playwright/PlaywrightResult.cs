using Microsoft.Playwright;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Playwright;

public class PlaywrightResult(PlaywrightOptions options) : IAsyncDisposable
{
    // ReSharper disable once InconsistentNaming
    internal IPage? _page;

    public IPage Page
    {
        get
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Playwright != null && Browser != null && _page == null) PlaywrightHelper.SetupPage(this, options).GetAwaiter().GetResult();
            // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

            return _page ?? throw new Exception("Unable to create Playwright page.");
        }
        set => _page = value;
    }

    public IPlaywright Playwright { get; set; } = null!;
    public IBrowser Browser { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_page != null) await _page.CloseAsync().ConfigureAwait(false);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Browser != null) await Browser.DisposeAsync().ConfigureAwait(false);

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        Playwright?.Dispose();
    }
}
