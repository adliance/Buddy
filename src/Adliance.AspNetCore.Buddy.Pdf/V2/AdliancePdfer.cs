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
            
            var parameters = new
            {
                html,
                footer = options.FooterHtml,
                footer_height = options.FooterHeight,
                header = options.HeaderHtml,
                header_height = options.HeaderHeight
            };
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");

            var endpoint = _configuration.ServerUrl.Trim('/');
            var response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}