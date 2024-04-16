using System;
using ZXing.QrCode.Internal;

namespace Adliance.Buddy.QrCode;

public class RenderConfiguration
{
    private const int FinderPatternSize = 7;
    private const float CircleScaleDownFactor = 0.5f;

    public required ByteMatrix Matrix;

    public required int Width;
    public required int Height;
    
    public int Margin { get; set; } = 4;
    
    private int QrWidth => Matrix.Width + (Margin * 2);
    private int QrHeight => Matrix.Height + (Margin * 2);
    
    public int OutputWidth => Math.Max(Width, QrWidth);

    public int OutputHeight => Math.Max(Height, QrHeight);
    
    public int Multiple => Math.Min(OutputWidth / QrWidth, OutputHeight / QrHeight);

    public int LeftPadding => (OutputWidth - Matrix.Width * Multiple) / 2;
    public int TopPadding => (OutputHeight - Matrix.Height * Multiple) / 2;

    public int DotSize => Math.Max((int)(Multiple * CircleScaleDownFactor), 1);
    public int PixelSize => Math.Max(Multiple, 1);
    
    public int FinderPatternDiameter => (int)(Multiple * FinderPatternSize / 1.9f);

    public int LeftTopFinderPatternX => LeftPadding;
    public int LeftTopFinderPatternY => TopPadding;
    
    public int RightTopFinderPatternX => LeftPadding + (Matrix.Width - FinderPatternSize) * Multiple + 5;
    public int RightTopFinderPatternY => TopPadding;
    
    public int LeftBottomFinderPatternX => LeftPadding;
    public int LeftBottomFinderPatternY => TopPadding + (Matrix.Height - FinderPatternSize) * Multiple + 5;
    
    public bool InFinderPatternRegion(int x, int y)
    {
        return x <= FinderPatternSize && y <= FinderPatternSize ||
               x >= Matrix.Width - FinderPatternSize && y <= FinderPatternSize ||
               x <= FinderPatternSize && y >= Matrix.Height - FinderPatternSize;
    }
}