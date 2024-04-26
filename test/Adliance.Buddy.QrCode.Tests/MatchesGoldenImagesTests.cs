using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Binarization;
using Xunit;

namespace Adliance.Buddy.QrCode.Tests;

public class MatchesGoldenImagesTests
{
    [Fact]
    public void AdlianceQrCodeMatchesGoldenImage()
    {
        var expected = Image.Load<Rgba32>("GoldenImages/simple-adliance.png");

        var actual = new QrCodeBuilder<Rgba32>("https://adliance.net")
            .Render();

        Assert.True(ImagesAreEqual(expected, actual));
    }

    [Fact]
    public void ComplexAdlianceQrCodeMatchesGoldenImage()
    {
        var expected = Image.Load<Rgba32>("GoldenImages/complex-adliance.png");

        var finderBrush = Brushes.Solid(new Color(new Rgb24(134, 184, 31)));

        var topCenter = new PointF(250, 0);
        var bottomCenter = new PointF(250, 500);
        var repetitionMode = GradientRepetitionMode.None;
        var topColor = new ColorStop(0.1f, new Color(new Rgb24(134, 184, 31)));
        var centerColor = new ColorStop(0.5f, new Color(new Rgb24(11, 176, 219)));
        var bottomColor = new ColorStop(0.9f, new Color(new Rgb24(18, 66, 146)));

        var contentBrush = new LinearGradientBrush(topCenter, bottomCenter,
            repetitionMode, topColor, centerColor, bottomColor);

        var adlianceLogo = Image.Load("adliance.png");
        var actual = new QrCodeBuilder<Rgba32>("https://adliance.net")
            .WithContentBrush(contentBrush)
            .WithFinderPatternBrush(finderBrush)
            .WithDimensions(500, 500)
            .WithSquircleFinderPattern()
            .WithSquircleContentDots()
            .WithOverlayImage(adlianceLogo)
            .WithOverlayMargin(1)
            .WithMargin(2)
            .Render();

        Assert.True(ImagesAreEqual(expected, actual));
    }

    [Fact]
    public void MadxQrCodeMatchesGoldenImage()
    {
        var expected = Image.Load<Rgba32>("GoldenImages/complex-madx.png");

        var madxFinderBrush = Brushes.Solid(new Color(new Rgba32(70, 69, 38)));
        var madxContentTopColor = new Color(new Rgba32(139, 137, 62));
        var madxContentBottomColor = new Color(new Rgba32(69, 68, 37));
        var madxContentBrush = new LinearGradientBrush(new PointF(250, 0), new PointF(250, 500),
            GradientRepetitionMode.None, new ColorStop(0.1f, madxContentTopColor), new ColorStop(0.9f, madxContentBottomColor));
        var madxOverlayBrush = new RecolorBrush(Color.Black, new Color(new Rgba32(188, 187, 74)), 1);

        var madxLogo = Image.Load("madx.png");
        madxLogo.Mutate(x =>
        {
            var binarizer = new BinaryThresholdProcessor(0.1f, Color.Black, Color.Transparent);
            x.ApplyProcessor(binarizer);
            x.Fill(new DrawingOptions()
            {
                GraphicsOptions = new GraphicsOptions()
                {
                    ColorBlendingMode = PixelColorBlendingMode.Screen,
                    AlphaCompositionMode = PixelAlphaCompositionMode.SrcAtop
                }
            }, madxOverlayBrush);
        });

        var actual = new QrCodeBuilder<Rgba32>("https://www.macroarraydx.com/products/systems")
            .WithDimensions(500, 500)
            .WithContentBrush(madxContentBrush)
            .WithFinderPatternBrush(madxFinderBrush)
            .WithOverlayImage(madxLogo)
            .Rounded()
            .Transparent()
            .WithMargin(2)
            .Render();

        Assert.True(ImagesAreEqual(expected, actual));
    }

    private bool ImagesAreEqual(Image<Rgba32> expected, Image<Rgba32> actual)
    {
        if (actual.Height != expected.Height || actual.Width != expected.Width) return false;

        for (var x = 0; x < actual.Width; x++)
        {
            for (var y = 0; y < actual.Height; y++)
            {
                if (!actual[x, y].Equals(expected[x, y]))
                {
                    return false;
                }
            }
        }

        return true;
    }
}