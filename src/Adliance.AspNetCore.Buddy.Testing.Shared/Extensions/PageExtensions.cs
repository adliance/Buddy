using Microsoft.Playwright;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Extensions;

public static class PageExtensions
{
    public static async Task Navigate(this IPage page, HttpClient client, string relativeUrl = "")
    {
        var baseUrl = client.BaseAddress?.ToString() ?? "";
        var url = baseUrl.TrimEnd('/') + "/" + relativeUrl.TrimStart('/');
        await page.GotoAsync(url);
    }

    public static async Task Screenshot(this IPage page, string fileName)
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

        var targetDirectory = Path.Combine("./", "screenshots");
        if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);
        var targetPath = Path.Combine(targetDirectory, fileName);

        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = targetPath,
            FullPage = true
        });
    }
}
