using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ZXing;
using ZXing.QrCode.Internal;

namespace Adliance.Buddy.QrCode;

public class QrCode<TPixel>(string content, int width, int height)
    where TPixel : unmanaged, IPixel<TPixel>
{
    public Image<TPixel> Render()
    {
        Dictionary<EncodeHintType, object> encodingHints = new Dictionary<EncodeHintType, object>
        {
            { EncodeHintType.CHARACTER_SET, "UTF-8" }
        };
        
        var code = Encoder.encode(content, ErrorCorrectionLevel.H, encodingHints);

        var renderConfig = new RenderConfiguration()
        {
            Matrix = code.Matrix,
            Height = height,
            Width = width
        };
        
        var image = new Image<TPixel>(width, height);
        
        image.Mutate(x => x.Fill(Color.White));

        RenderContent(renderConfig, image);
        RenderRoundedFinderPattern(renderConfig, image);

        return image;
    }

    private void RenderContent(RenderConfiguration configuration, Image<TPixel> image)
    {
        for (int inputY = 0, outputY = configuration.TopPadding;
             inputY < configuration.Matrix.Height;
             inputY++, outputY += configuration.Multiple)
        {
            for (int inputX = 0, outputX = configuration.LeftPadding;
                 inputX < configuration.Matrix.Width;
                 inputX++, outputX += configuration.Multiple)
            {
                if (configuration.Matrix[inputX, inputY] != 1) continue;

                switch (configuration.Rounded)
                {
                    case true when !configuration.InFinderPatternRegion(inputX, inputY):
                    {
                        var circle = new EllipsePolygon(outputX + configuration.DotSize, outputY + configuration.DotSize, configuration.DotSize);
                        image.Mutate(x => x.Fill(Color.Black, circle));
                        break;
                    }
                    case false:
                    {
                        var rect = new RectangularPolygon(outputX, outputY, configuration.PixelSize, configuration.PixelSize);
                        image.Mutate(x => x.Fill(Color.Black, rect));
                        break;
                    }
                }
            }
        }
    }

    private void RenderRoundedFinderPattern(RenderConfiguration configuration, Image<TPixel> image)
    {
        if (!configuration.Rounded) return;
        DrawFinderPatternCircleStyle(image, configuration.LeftTopFinderPatternX, configuration.LeftTopFinderPatternY, configuration.FinderPatternDiameter);
        DrawFinderPatternCircleStyle(image, configuration.RightTopFinderPatternX, configuration.RightTopFinderPatternY, configuration.FinderPatternDiameter);
        DrawFinderPatternCircleStyle(image, configuration.LeftBottomFinderPatternX, configuration.LeftBottomFinderPatternY, configuration.FinderPatternDiameter);
    }
    
    private static void DrawFinderPatternCircleStyle(Image<TPixel> image, int x, int y, int circleDiameter)
    {
        var whiteCircleDiameter = 5 * circleDiameter / 7;
        var middleDotDiameter = 3 * circleDiameter / 7;

        image.Mutate(img =>
        {
            var circle = new EllipsePolygon(x, y, circleDiameter);
            img.Fill(Color.Black, circle);
            circle = new EllipsePolygon(x, y, whiteCircleDiameter);
            img.Fill(Color.White, circle);
            circle = new EllipsePolygon(x, y, middleDotDiameter);
            img.Fill(Color.Black, circle);
        });
    }
}