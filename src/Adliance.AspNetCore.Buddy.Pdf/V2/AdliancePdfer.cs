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
        ThrowIfServerConfigurationIsInvalid();

        var paperSize = CalculatePaperSize(options.Size, options.PaperWidth, options.PaperHeight);
        var parameters = new
        {
            html,
            footer = options.FooterHtml,
            footer_height = options.FooterHeight,
            header = options.HeaderHtml,
            header_height = options.HeaderHeight,
            paper_width = paperSize[0],
            paper_height = paperSize[1],
            print_background = options.PrintBackground,
            scale = options.Scale
        };
        var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
        var endpoint = configuration.ServerUrl.Trim('/');

        return await GeneratePdfWithRetries(endpoint, content);
    }

    public async Task<byte[]> TemplateToPdf(TemplateOptions body, HeaderTemplateOptions? header, FooterTemplateOptions? footer, PdfOptions options)
    {
        ThrowIfServerConfigurationIsInvalid();

        var paperSize = CalculatePaperSize(options.Size, options.PaperWidth, options.PaperHeight);
        var parameters = new
        {
            template = body.Template,
            model = body.Model,
            js = body.Javascript,
            header_template = header?.Template,
            header_model = header?.Model,
            header_js = header?.Javascript,
            header_height = header?.Height,
            footer_template = footer?.Template,
            footer_model = footer?.Model,
            footer_js = footer?.Javascript,
            footer_height = footer?.Height,
            paper_width = paperSize[0],
            paper_height = paperSize[1],
            print_background = options.PrintBackground,
            scale = options.Scale
        };
        //TODO: serializer options for naming?
        var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
        var endpoint = $"{configuration.ServerUrl.Trim('/')}/template";

        return await GeneratePdfWithRetries(endpoint, content);
    }

    private void ThrowIfServerConfigurationIsInvalid()
    {
        if (string.IsNullOrWhiteSpace(configuration.ServerUrl))
        {
            throw new Exception("No Server URL configured.");
        }
    }

    private static async Task<byte[]> GeneratePdfWithRetries(string endpoint, StringContent content)
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(1);

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
