using Xunit;
using ZXing;
using ZXing.QrCode.Internal;

namespace Adliance.Buddy.QrCode.Tests;

public class RenderConfigurationTests
{

    [Theory]
    [InlineData("https://adliance.net", 500, 4, 25, 15, 120, 4 * 15)]
    [InlineData("https://adliance.net/jobs/software-engineer", 500, 4, 29, 14, 126, 4 * 14)]
    public void ProducesCorrectSizes(string content, int size, int margin, int expectedMatrixColumnCount, int expectedPixelRatio, int expectedCutoutOutputSize, int expectedOutputOffset)
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
        Assert.Equal(expectedPixelRatio, renderConfig.PixelRatio);
        Assert.Equal(expectedOutputOffset, renderConfig.LeftPadding);
        Assert.Equal(expectedOutputOffset, renderConfig.TopPadding);
        Assert.Equal(expectedCutoutOutputSize, renderConfig.GetCutoutSize(code, 0).Width);
    }
}