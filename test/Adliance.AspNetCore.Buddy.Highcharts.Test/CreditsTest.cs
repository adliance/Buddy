using System.Threading.Tasks;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Highcharts.Test;

public class CreditsTest
{
    private readonly HighchartsServer _server;

    public CreditsTest()
    {
        _server = new HighchartsServer(new HighchartsServerDefaultSettings());
    }

    [Fact]
    public async Task Can_Configure_Credits()
    {
        var chart = new Chart
        {
            Options = new Options
            {
                Height = 500,
                Width = 1000
            },
            Credits = new Credits
            {
                Enabled = true,
                Href = "https://www.adliance.net",
                Text = "Das sind die Credits",
                Style = new Style
                {
                    Color = "green",
                    FontFamily = "Courier New, monospace",
                    FontSize = "20px"
                },
                Position = new Position
                {
                    X = -10,
                    Y = -50
                }
            }
        };

        var bytes = await _server.Render(new HighchartsServerParameter
        {
            Chart = chart,
            Format = "png"
        });

        TestUtils.StoreLocally(@"chart_credits.png", bytes);
    }
}
