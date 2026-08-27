using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using ZXing;
using ZXing.Common;

namespace Adliance.Buddy.QrCode.Tests;

/// <summary>
/// Compatibility probe for ZXing.Net.Bindings.ImageSharp.V4.
/// The V4 bindings are compiled against SixLabors.ImageSharp 4.x, which is what we use, so both the renderer and the
/// luminance source work. These tests pin that down: when they start failing, the bindings have broken against our
/// ImageSharp version again and QrCodeBuilder can no longer be built on top of them.
/// </summary>
public class ZXingImageSharpBindingsTests
{
    [Fact]
    public void GeneratingQrCodeWorks()
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

        using var image = writer.Write("https://adliance.net");

        Assert.Equal(200, image.Width);
        Assert.Equal(200, image.Height);
        Assert.Equal("https://adliance.net", Decode(image));
    }

    [Fact]
    public void ReadingQrCodeWorks()
    {
        using var image = new QrCodeBuilder<Rgba32>("https://adliance.net")
            .WithDimensions(200, 200)
            .Render();

        Assert.Equal("https://adliance.net", Decode(image));
    }

    private static string? Decode(SixLabors.ImageSharp.Image<Rgba32> image)
    {
        var reader = new ZXing.ImageSharp.BarcodeReader<Rgba32>
        {
            Options = new DecodingOptions
            {
                PossibleFormats = [BarcodeFormat.QR_CODE]
            }
        };

        return reader.Decode(image)?.Text;
    }
}
