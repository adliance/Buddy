using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Highcharts.Extensions;
using Newtonsoft.Json;

namespace Adliance.AspNetCore.Buddy.Highcharts
{
    public class HighchartsServer
    {
        private readonly IHighchartsServerSettings _settings;

        public HighchartsServer(IHighchartsServerSettings settings)
        {
            _settings = settings;
        }

        public async Task<byte[]> Render(HighchartsServerParameter parameter)
        {
            using (var client = new HttpClient())
            {
                var inputJson = parameter.ToJson(Formatting.Indented);
                var response = await client.PostAsync(_settings.HighchartsServerUrl, new StringContent(inputJson, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var responseBytes = await response.Content.ReadAsByteArrayAsync();
                    return responseBytes;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                throw new Exception($"Unable to render chart on server (Status code: {(int)response.StatusCode}): {responseString}");
            }
        }
    }
}
