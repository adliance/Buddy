using System;
using SixLabors.ImageSharp;
using ZXing.QrCode.Internal;

namespace Adliance.Buddy.QrCode;

internal class RenderConfiguration
{
    private const int FinderPatternSize = 7;
    private const float CircleScaleDownFactor = 0.5f;

    public required ByteMatrix Matrix { get; set; }

    public required int Width { get; set; }
    public required int Height { get; set; }

    public int Margin { get; set; } = 4;

    private int QrWidth => Matrix.Width + (Margin * 2);
    private int QrHeight => Matrix.Height + (Margin * 2);

    private int OutputWidth => Math.Max(Width, QrWidth);

    private int OutputHeight => Math.Max(Height, QrHeight);

    public int PixelRatio => (int)Math.Min(Math.Round(OutputWidth / (decimal)QrWidth), Math.Round(OutputHeight / (decimal)QrHeight));

    public int LeftPadding => Margin * PixelRatio;
    public int TopPadding => Margin * PixelRatio;

    public int DotSize => Math.Max((int)(PixelRatio * CircleScaleDownFactor), 1);
    public int PixelSize => Math.Max(PixelRatio, 1);

    public int FinderPatternDiameter => (int)(PixelRatio * FinderPatternSize / 1.9f);
    public int FinderPatternOutputSize => PixelRatio * FinderPatternSize;

    public int LeftTopFinderPatternX => LeftPadding;
    public int LeftTopFinderPatternY => TopPadding;

    public int RightTopFinderPatternX => LeftPadding + (Matrix.Width - FinderPatternSize) * PixelRatio;
    public int RightTopFinderPatternY => TopPadding;

    public int LeftBottomFinderPatternX => LeftPadding;
    public int LeftBottomFinderPatternY => TopPadding + (Matrix.Height - FinderPatternSize) * PixelRatio;

    public bool InFinderPatternRegion(int x, int y)
    {
        return x <= FinderPatternSize && y <= FinderPatternSize ||
               x >= Matrix.Width - FinderPatternSize && y <= FinderPatternSize ||
               x <= FinderPatternSize && y >= Matrix.Height - FinderPatternSize;
    }

    public Size GetCutoutSize(QRCode code, int margin)
    {
        // margin * 2 for both sides
        var columnCount = code.Matrix.Width / 3 + margin * 2;
        return new Size(columnCount * PixelRatio);
    }

    public Size GetOverlayImageSize(QRCode code)
    {
        return GetCutoutSize(code, 0);
    }
}