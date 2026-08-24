using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using ZXing;
using ZXing.Common;

namespace Adliance.Buddy.QrCode.Tests;

/// <summary>
/// Compatibility probe for ZXing.Net.Bindings.ImageSharp.
/// The bindings (0.16.16) are compiled against SixLabors.ImageSharp 1.0.4, we use 4.x. Both the renderer and the
/// luminance source call ImageSharp members that no longer exist, so the bindings cannot be used at all right now.
/// These tests pin down that failure: when they start failing, the bindings have become usable (or broke differently)
/// and QrCodeBuilder could be built on top of them instead of on the raw ZXing.Net encoder.
/// </summary>
public class ZXingImageSharpBindingsTests
{
    [Fact]
    public void GeneratingQrCodeFailsBecauseBindingsExpectOlderImageSharp()
    {
        var writer = new ZXing.ImageSharp.BarcodeWriter<Rgba32>
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Width = 200,
                Height = 200,
                Margin = 1
            }
        };

        // ImageSharpRenderer<TPixel> converts Color to TPixel via an implicit operator that was removed after ImageSharp 1.x.
        var exception = Assert.Throws<MissingMethodException>(() => writer.Write("https://adliance.net"));
        Assert.Contains("Rgba32.op_Implicit", exception.Message);
    }

    [Fact]
    public void ReadingQrCodeFailsBecauseBindingsExpectOlderImageSharp()
    {
        using var image = new QrCodeBuilder<Rgba32>("https://adliance.net")
            .WithDimensions(200, 200)
            .Render();

        var reader = new ZXing.ImageSharp.BarcodeReader<Rgba32>
        {
            Options = new DecodingOptions
            {
                PossibleFormats = [BarcodeFormat.QR_CODE]
            }
        };

        // ImageSharpLuminanceSource<TPixel> reads pixels via Image<TPixel>.GetPixelRowSpan(), which was removed in ImageSharp 2.x.
        var exception = Assert.Throws<MissingMethodException>(() => reader.Decode(image));
        Assert.Contains("GetPixelRowSpan", exception.Message);
    }
}
