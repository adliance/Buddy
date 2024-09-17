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

    public float PixelRatio => Math.Min(OutputWidth / (float)QrWidth, OutputHeight / (float)QrHeight);

    public int LeftPadding => (int)Math.Round(Margin * PixelRatio);
    public int TopPadding => (int)Math.Round(Margin * PixelRatio);

    public int DotSize => Math.Max((int)(PixelRatio * CircleScaleDownFactor), 1);
    public int PixelSize => (int)Math.Max(PixelRatio, 1);

    public float FinderPatternDiameter => PixelRatio * FinderPatternSize / 1.9f;
    public float FinderPatternOutputSize => PixelRatio * FinderPatternSize;

    public int LeftTopFinderPatternX => LeftPadding;
    public int LeftTopFinderPatternY => TopPadding;

    public int RightTopFinderPatternX => (int)(LeftPadding + (Matrix.Width - FinderPatternSize) * PixelRatio);
    public int RightTopFinderPatternY => TopPadding;

    public int LeftBottomFinderPatternX => LeftPadding;
    public int LeftBottomFinderPatternY => (int)(TopPadding + (Matrix.Height - FinderPatternSize) * PixelRatio);

    public bool InFinderPatternRegion(int x, int y)
    {
        return x <= FinderPatternSize && y <= FinderPatternSize ||
               x >= Matrix.Width - FinderPatternSize && y <= FinderPatternSize ||
               x <= FinderPatternSize && y >= Matrix.Height - FinderPatternSize;
    }

    public Size GetCutoutSize(int overlayMargin)
    {
        // margin * 2 for both sides
        var portion = Matrix.Width > 28 ? 3f : 4f;
        var columnCount = Math.Ceiling(QrWidth / portion) + overlayMargin * 2;
        if (QrWidth % 2 != columnCount % 2)
        {
            columnCount--;
        }
        return new Size((int)Math.Round(columnCount * PixelRatio));
    }

    public Size GetOverlayImageSize()
    {
        return GetCutoutSize(0);
    }
}