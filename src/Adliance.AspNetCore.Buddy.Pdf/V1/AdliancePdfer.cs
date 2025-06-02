using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.V1;

public class AdliancePdfer(IPdferConfiguration configuration) : IPdfer
{
    public async Task<byte[]> HtmlToPdf(string html, PdfOptions options)
    {
        if (string.IsNullOrWhiteSpace(configuration.ServerUrl))
        {
            throw new Exception("No Server URL configured.");
        }

        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(1);

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

        var endpoint = configuration.ServerUrl.Trim('/');
        if (!endpoint.EndsWith("/html-to-pdf"))
        {
            endpoint += "/html-to-pdf";
        }

        var backoffMs = 2000;
        while (true)
        {
            try
            {
                var response = await client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                if (backoffMs < 10000)
                {
                    Thread.Sleep(backoffMs);
                    backoffMs *= 2;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
