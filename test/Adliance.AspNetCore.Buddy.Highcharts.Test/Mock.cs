namespace Adliance.AspNetCore.Buddy.Highcharts.Test;

public class MockedHighchartsServerSettings : HighchartsServerDefaultSettings
{
    public MockedHighchartsServerSettings()
    {
        HighchartsServerUrl = "https://highcharts.adliance.dev";
    }
}