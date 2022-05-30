using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.V2
{
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

            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(3)
            };

            var paperSize = ConvertPaperSizeFromMMToPixels(options.Size, options.PaperWidth, options.PaperHeight);
            
            var parameters = new
            {
                html,
                footer = options.FooterHtml,
                footer_height = options.FooterHeight,
                header = options.HeaderHtml,
                header_height = options.HeaderHeight,
                paperWidth = paperSize[0],
                paperHeight = paperSize[1]
            };
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");

            var endpoint = _configuration.ServerUrl.Trim('/');
            var response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        
        private int[] ConvertPaperSizeFromMMToPixels(PdfSize? size, int? paperWidth, int? paperHeight)
        {
            if (paperWidth.HasValue && paperHeight.HasValue)
            {
                // we already have width and height in pixels, return them
                return new[] { (int)paperWidth, (int)paperHeight };
            }

            return size switch
            {
                PdfSize.A2 => new[] { 1587, 2245 },
                PdfSize.A3 => new[] { 1122, 1587 },
                PdfSize.A4 => new[] { 794, 1122 },
                PdfSize.A5 => new[] { 559, 794 },
                PdfSize.A6 => new[] { 397, 559 },
                PdfSize.Letter => new[] { 816, 1056 },
                _ => new[] { 794, 1122 }
            };
        }
    }
}