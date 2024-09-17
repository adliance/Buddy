using Xunit;
using ZXing;
using ZXing.QrCode.Internal;

namespace Adliance.Buddy.QrCode.Tests;

public class RenderConfigurationTests
{

    [Theory]
    [InlineData("https://adliance.net", 500, 4, 25, 15.15, 136)] // round(ceil((25 + 2*4)/4) * 15.15)
    [InlineData("https://adliance.net", 200, 0, 25, 8, 56)] // round(ceil(25/4) * 8)
    [InlineData("https://adliance.net/jobs/software-engineer", 500, 4, 29, 13.51, 176)] // round(ceil((29 + 2*4)/3) * 13.51)
    [InlineData("https://adliance.net/jobs/software-engineer", 500, 2, 29, 15.15, 167)] // round(ceil((29 + 2*2)/3) * 15.15)
    [InlineData("https://p.madx.com/m/onpWGXPGP4oUPS3K7yBQmUfmxVyjBN", 500, 1, 29, 16.13, 177)] // round(ceil((29 + 2*1)/3) * 16.13)
    [InlineData("https://p.madx.com/m/onpWGXPGP4oUPS3K7yBQmUfmxVyjBN", 500, 2, 29, 15.15, 167)] // round(ceil((29 + 2*2)/3) * 15.15)
    public void ProducesCorrectSizes(string content, int size, int margin, int expectedMatrixColumnCount, float expectedPixelRatio, int expectedCutoutOutputSize)
    {
        Dictionary<EncodeHintType, object> encodingHints = new Dictionary<EncodeHintType, object>
        {
            { EncodeHintType.CHARACTER_SET, "UTF-8" }
        };

        var code = Encoder.encode(content, ErrorCorrectionLevel.L, encodingHints);

        var renderConfig = new RenderConfiguration()
        {
            Matrix = code.Matrix,
            Height = size,
            Width = size,
            Margin = margin
        };
        
        Assert.Equal(expectedMatrixColumnCount, code.Matrix.Width);
        Assert.Equal(expectedPixelRatio, renderConfig.PixelRatio, 2);
        Assert.Equal(expectedPixelRatio * margin, renderConfig.LeftPadding, 0);
        Assert.Equal(expectedPixelRatio * margin, renderConfig.TopPadding, 0);
        Assert.Equal(expectedCutoutOutputSize, renderConfig.GetCutoutSize(0).Width);
        // Assert full columns plus/minus one pixel
        var deviation = renderConfig.GetCutoutSize(0).Width % expectedPixelRatio;
        Assert.Equal(deviation > expectedPixelRatio / 2 ? expectedPixelRatio : 0f, deviation, 0);
    }
}