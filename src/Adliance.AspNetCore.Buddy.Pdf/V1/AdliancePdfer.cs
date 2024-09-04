using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.V1;

public class AdliancePdfer : IPdfer
{
    private readonly IPdferConfiguration _configuration;

    public AdliancePdfer(IPdferConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<byte[]> HtmlToPdf(string html, PdfOptions options)
    {
        if (string.IsNullOrWhiteSpace(_configuration.ServerUrl))
        {
            throw new Exception("No Server URL configured.");
        }

        using (var client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(3)
        })
        {
            var parameters = new
            {
                html,
                marginLeft = options.MarginLeft.ToString(CultureInfo.InvariantCulture),
                marginRight = options.MarginRight.ToString(CultureInfo.InvariantCulture),
                marginTop = options.MarginTop.ToString(CultureInfo.InvariantCulture),
                marginBottom = options.MarginBottom.ToString(CultureInfo.InvariantCulture),
                headerHtml = options.HeaderHtml ?? "",
                footerHtml = options.FooterHtml ?? "",
                headerSpacing = options.HeaderSpacing.ToString(CultureInfo.InvariantCulture),
                footerSpacing = options.FooterSpacing.ToString(CultureInfo.InvariantCulture),

                size = options.Size.ToString(),
                orientation = options.Orientation.ToString(),

                useCompression = options.UseCompression,
                dpi = options.Dpi,
                imageDpi = options.ImageDpi,
                imageQuality = options.ImageQuality,

                useSmartShrinking = options.UseSmartShrinking,
                enableJavaScript = options.EnableJavaScript,
                loadImages = options.LoadImages,
                printBackground = options.PrintBackground,
                printMediaType = options.PrintMediaType,
                enablePlugins = options.EnablePlugins
            };
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");

            var endpoint = _configuration.ServerUrl.Trim('/');
            if (!endpoint.EndsWith("/html-to-pdf"))
            {
                endpoint += "/html-to-pdf";
            }

            var response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
