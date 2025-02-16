using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Adliance.AspNetCore.Buddy.Testing.Playwright;

public static class PlaywrightHelper
{
    public static async Task<PlaywrightResult> Setup(PlaywrightOptions options)
    {
        var result = new PlaywrightResult();
        result.Playwright = await Microsoft.Playwright.Playwright.CreateAsync().ConfigureAwait(false);
        return await SetupBrowser(result, options).ConfigureAwait(false);
    }

    public static async Task<PlaywrightResult> SetupBrowser(PlaywrightResult result, PlaywrightOptions options)
    {
        if (result.Playwright == null) throw new Exception("Playwright is not initialized.");

        // ReSharper disable RedundantAlwaysMatchSubpattern
        if (result is { Browser: not null, Page: not null }) await result.Page.CloseAsync().ConfigureAwait(false);
        // ReSharper restore RedundantAlwaysMatchSubpattern

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (result.Browser != null) await result.Browser.DisposeAsync().ConfigureAwait(false);

        result.Browser = await result.Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = options.Headless
        });

        return result;
    }

    public static async Task<PlaywrightResult> SetupPage(PlaywrightResult result)
    {
        if (result.Browser == null) throw new Exception("Browser is not initialized.");

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (result._page != null) await result._page.CloseAsync().ConfigureAwait(false);

        result.Page = await result.Browser.NewPageAsync(new BrowserNewPageOptions
        {
            Locale = "en-US",
            ScreenSize = new ScreenSize
            {
                Height = 1000,
                Width = 1200
            }
        });

        return result;
    }
}
