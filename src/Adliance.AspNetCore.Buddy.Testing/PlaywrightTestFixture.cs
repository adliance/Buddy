using Microsoft.Playwright;

namespace TestsPoC.Web.Tests;

public class PlaywrightTestFixture : TestFixture
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    public IPage Page { get; set; } = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        Page = await _browser.NewPageAsync(new BrowserNewPageOptions
        {
            Locale = "en-US",
            ScreenSize = new ScreenSize
            {
                Height = 1000,
                Width = 1200
            }
        });
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync().ConfigureAwait(false);
        await _browser.DisposeAsync().ConfigureAwait(false);
        _playwright.Dispose();
        await base.DisposeAsync();
    }

    public async Task Navigate(string relativeUrl)
    {
        var url = base.BaseUrl.TrimEnd('/') + "/" + relativeUrl.TrimStart('/');
        await Page.GotoAsync(url);
    }

    public async Task Screenshot(string fileName)
    {
        if (!Path.GetExtension(fileName).Equals(".png", StringComparison.OrdinalIgnoreCase)) fileName += ".png";

        // remove special characters from filename, as Azure DevOps cannot handle them
        var invalidCharacters = Path.GetInvalidPathChars().ToList();
        invalidCharacters.AddRange(Path.GetInvalidFileNameChars());
        invalidCharacters.AddRange([
            ':',
            '/',
            '\\'
        ]);
        fileName = invalidCharacters.Aggregate(fileName, (current, c) => current.Replace(c, '_'));

        var targetPath = Path.Combine("./", fileName);
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = targetPath,
            FullPage = true
        });
    }
}
