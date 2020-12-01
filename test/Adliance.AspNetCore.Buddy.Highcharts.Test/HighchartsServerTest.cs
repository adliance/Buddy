using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Highcharts.Test
{
    public class HighchartsServerTest
    {
        private readonly HighchartsServer _server;

        public HighchartsServerTest()
        {
            _server = new HighchartsServer(new HighchartsServerDefaultSettings
            {
                HighchartsServerUrl = "https://adliance-highcharts-server.azurewebsites.net"
            });
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public async Task Can_Generate_Charts(string filename, string format, int? width, double? scale)
        {
            var js = TestUtils.GetEmbeddedString("Charts", filename + ".js");

            var bytes = await _server.Render(new HighchartsServerParameter
            {
                Format = format,
                Callback = js,
                Width = width,
                Scale = scale,
                Chart = JsonConvert.DeserializeObject<Chart>(TestUtils.GetEmbeddedString("Charts", filename + ".json"))
            });

            TestUtils.StoreLocally($@"chart_{filename}_{format}_{width}_{scale}.{format}", bytes);
            Assert.True(bytes.Length > 1000);
        }

        public class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var filename in new[] {"Chart01"})
                {
                    foreach (var format in new[] {"png", "svg", "jpg", "pdf"})
                    {
                        yield return new object[] {filename, format, null!, null!};
                        yield return new object[] {filename, format, 100, null!};
                        yield return new object[] {filename, format, null!, 5.5};
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}