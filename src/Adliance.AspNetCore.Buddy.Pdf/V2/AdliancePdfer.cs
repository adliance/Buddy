using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public class AdliancePdfer(IPdferConfiguration configuration) : IPdfer
{
    public async Task<byte[]> HtmlToPdf(string html, PdfOptions options)
    {
        var paperSize = CalculatePaperSize(options.Size, options.PaperWidth, options.PaperHeight);

        var parameters = new
        {
            // ReSharper disable RedundantAnonymousTypePropertyName
            html = html,
            // ReSharper restore RedundantAnonymousTypePropertyName
            footer = options.FooterHtml,
            header = options.HeaderHtml,

            header_height = options.HeaderHeight,
            footer_height = options.FooterHeight,
            paper_width = paperSize[0],
            paper_height = paperSize[1],
            print_background = options.PrintBackground,
            outline = options.Outline,
            scale = options.Scale
        };

        return await Send("/", configuration.ApiKeyPdf, parameters);
    }

    public async Task<PdfMetadata> GetPdfMetadata(byte[] pdfBytes)
    {
        var requestContent = new ByteArrayContent(pdfBytes);
        var requestUrl = $"{configuration.ServerUrl.TrimEnd('/')}/pdf-metadata";
        var response = await PostAndBackoff(configuration.ApiKeyPdf, requestUrl, requestContent);
        var responseString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseString)) return new PdfMetadata();

        try
        {
            return JsonSerializer.Deserialize<PdfMetadata>(responseString) ?? throw new Exception("Unable to deserialize.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Could not deserialize JSON: {responseString}", ex);
        }
    }

    public async Task<byte[]> TemplateToPdf(string template, object model, TemplateOptions options)
    {
        var paperSize = CalculatePaperSize(options.Size, options.PaperWidth, options.PaperHeight);

        var parameters = new
        {
            // ReSharper disable RedundantAnonymousTypePropertyName
            template = template,
            model = model,
            // ReSharper restore RedundantAnonymousTypePropertyName
            js = options.JavaScript,
            header_template = options.HeaderHtml,
            header_model = options.HeaderModel,
            header_js = options.HeaderJavaScript,
            footer_template = options.FooterHtml,
            footer_model = options.FooterModel,
            footer_js = options.FooterJavaScript,

            header_height = options.HeaderHeight,
            footer_height = options.FooterHeight,
            paper_width = paperSize[0],
            paper_height = paperSize[1],
            print_background = options.PrintBackground,
            scale = options.Scale
        };

        return await Send("/template", configuration.ApiKeyTemplate, parameters);
    }

    private async Task<byte[]> Send(string endpoint, string? apiKey, object parameters)
    {
        var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
        endpoint = $"{configuration.ServerUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

        var response = await PostAndBackoff(apiKey, endpoint, content);
        return await response.Content.ReadAsByteArrayAsync();
    }

    private async Task<HttpResponseMessage> PostAndBackoff(string? apiKey, string url, HttpContent content)
    {
        using var client = GetClient(apiKey);

        var backoffMs = 2000;
        while (true)
        {
            try
            {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return response;
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

    private HttpClient GetClient(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(configuration.ServerUrl)) throw new Exception("No Server URL configured.");

        var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(1);
        if (!string.IsNullOrWhiteSpace(apiKey)) client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        return client;
    }

    /// <summary>
    /// Paper size can be either set via enum size" or directly via width and height properties.
    /// If paperWidth and paperHeight are set, those values are used. Otherwise the size parameter is used.
    /// As fallback always "A4" is used.
    /// </summary>
    /// <param name="size">The paper size as PdfSize.</param>
    /// <param name="paperWidth">The width in pixels.</param>
    /// <param name="paperHeight">The height in pixels.</param>
    /// <returns></returns>
    private static int[] CalculatePaperSize(PdfSize? size, int? paperWidth, int? paperHeight)
    {
        if (paperWidth.HasValue && paperHeight.HasValue)
        {
            // we already have width and height in pixels, return them
            return [(int)paperWidth, (int)paperHeight];
        }

        return size switch
        {
            PdfSize.A2 => [1587, 2245],
            PdfSize.A3 => [1122, 1587],
            PdfSize.A4 => [794, 1122],
            PdfSize.A5 => [559, 794],
            PdfSize.A6 => [397, 559],
            PdfSize.Letter => [816, 1056],
            _ => [794, 1122]
        };
    }
}
