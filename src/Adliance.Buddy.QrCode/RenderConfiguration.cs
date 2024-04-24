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

    public int Multiple => Math.Min(OutputWidth / QrWidth, OutputHeight / QrHeight);

    public int LeftPadding => (OutputWidth - Matrix.Width * Multiple) / 2;
    public int TopPadding => (OutputHeight - Matrix.Height * Multiple) / 2;

    public int DotSize => Math.Max((int)(Multiple * CircleScaleDownFactor), 1);
    public int PixelSize => Math.Max(Multiple, 1);

    public int FinderPatternDiameter => (int)(Multiple * FinderPatternSize / 1.9f);
    public int FinderPatternOutputSize => Multiple * FinderPatternSize;

    public int LeftTopFinderPatternX => LeftPadding;
    public int LeftTopFinderPatternY => TopPadding;

    public int RightTopFinderPatternX => LeftPadding + (Matrix.Width - FinderPatternSize) * Multiple;
    public int RightTopFinderPatternY => TopPadding;

    public int LeftBottomFinderPatternX => LeftPadding;
    public int LeftBottomFinderPatternY => TopPadding + (Matrix.Height - FinderPatternSize) * Multiple;

    public bool InFinderPatternRegion(int x, int y)
    {
        return x <= FinderPatternSize && y <= FinderPatternSize ||
               x >= Matrix.Width - FinderPatternSize && y <= FinderPatternSize ||
               x <= FinderPatternSize && y >= Matrix.Height - FinderPatternSize;
    }

    public Size GetCutoutSize(QRCode code, int margin)
    {
        var columnCount = code.Matrix.Width / 3 + margin;
        return new Size(columnCount * Multiple);
    }

    public Size GetOverlayImageSize(QRCode code)
    {
        return GetCutoutSize(code, 0);
    }
}