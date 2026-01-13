using Microsoft.Playwright;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Extensions;

public static class PageExtensions
{
    public static async Task<IResponse?> Navigate(this IPage page, HttpClient client, string relativeUrl = "", WaitUntilState waitUntil = WaitUntilState.Load)
    {
        var baseUrl = client.BaseAddress?.ToString() ?? "";
        var url = baseUrl.TrimEnd('/') + "/" + relativeUrl.TrimStart('/');
        var options = new PageGotoOptions
        {
            WaitUntil = waitUntil
        };
        return await page.GotoAsync(url, options);
    }

    public static async Task<byte[]> Screenshot(this IPage page, string fileName)
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

        return await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = targetPath,
            FullPage = true
        });
    }
}
