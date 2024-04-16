using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ZXing;
using ZXing.QrCode.Internal;

namespace Adliance.Buddy.QrCode;

public class QrCodeBuilder<TPixel>(string content)
    where TPixel : unmanaged, IPixel<TPixel>
{
    private int _width = 250;
    private int _height = 250;

    private ErrorCorrectionLevel _errorCorrectionLevel = ErrorCorrectionLevel.L;
    
    private bool _roundedFinderPattern = false;
    private bool _roundedContentDots = false;

    private Brush _finderPatternBrush = Brushes.Solid(Color.Black);
    private Brush _contentBrush = Brushes.Solid(Color.Black);

    private Image? _overlayImage;

    public QrCodeBuilder<TPixel> SetDimensions(int width, int height)
    {
        if (width < 0 || height < 0) throw new ArgumentException("Height and width must be greater than 0.");

        _width = width;
        _height = height;

        return this;
    }

    public QrCodeBuilder<TPixel> WithErrorCorrectionLevel(ErrorCorrectionLevel level)
    {
        _errorCorrectionLevel = level;
        return this;
    }

    public QrCodeBuilder<TPixel> WithFinderPatternBrush(Brush brush)
    {
        _finderPatternBrush = brush;
        return this;
    }
    
    public QrCodeBuilder<TPixel> WithContentBrush(Brush brush)
    {
        _contentBrush = brush;
        return this;
    }
    
    public QrCodeBuilder<TPixel> Rounded()
    {
        WithRoundedFinderPattern();
        return WithRoundedContentDots();
    }
    
    public QrCodeBuilder<TPixel> WithRoundedFinderPattern()
    {
        _roundedFinderPattern = true;
        return this;
    }
    
    public QrCodeBuilder<TPixel> WithRoundedContentDots()
    {
        _roundedContentDots = true;
        return this;
    }
    
    public QrCodeBuilder<TPixel> WithOverlayImage(Image image)
    {
        _overlayImage = image;
        if (_errorCorrectionLevel == ErrorCorrectionLevel.L || _errorCorrectionLevel == ErrorCorrectionLevel.M)
        {
            _errorCorrectionLevel = ErrorCorrectionLevel.Q;
        }
        return this;
    }
    
    public Image<TPixel> Render()
    {
        Dictionary<EncodeHintType, object> encodingHints = new Dictionary<EncodeHintType, object>
        {
            { EncodeHintType.CHARACTER_SET, "UTF-8" }
        };

        var code = Encoder.encode(content, _errorCorrectionLevel, encodingHints);

        var renderConfig = new RenderConfiguration()
        {
            Matrix = code.Matrix,
            Height = _height,
            Width = _width
        };

        var image = new Image<TPixel>(_width, _height);

        image.Mutate(x => x.Fill(Color.White));

        RenderContent(renderConfig, image);
        RenderFinderPattern(renderConfig, image);

        if (_overlayImage != null)
        {
            _overlayImage.Mutate(x => x.Resize(_width/4, _height/4));
            var location = new Point((_width - _overlayImage.Width) / 2, (_height - _overlayImage.Height) / 2);
            image.Mutate(x => x.DrawImage(_overlayImage, location, 1f));
        }

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
                if (configuration.InFinderPatternRegion(inputX, inputY)) continue;

                if (_roundedContentDots)
                {
                    var circle = new EllipsePolygon(outputX + configuration.DotSize, outputY + configuration.DotSize,
                        configuration.DotSize);
                    image.Mutate(x => x.Fill(_contentBrush, circle));
                }
                else
                {
                    var rect = new RectangularPolygon(outputX, outputY, configuration.PixelSize,
                        configuration.PixelSize);
                    image.Mutate(x => x.Fill(_contentBrush, rect));
                }
            }
        }
    }

    private void RenderFinderPattern(RenderConfiguration configuration, Image<TPixel> image)
    {
        if (_roundedFinderPattern)
        {
            DrawFinderPatternCircleStyle(image, configuration.LeftTopFinderPatternX,
                configuration.LeftTopFinderPatternY, configuration.FinderPatternDiameter);
            DrawFinderPatternCircleStyle(image, configuration.RightTopFinderPatternX,
                configuration.RightTopFinderPatternY, configuration.FinderPatternDiameter);
            DrawFinderPatternCircleStyle(image, configuration.LeftBottomFinderPatternX,
                configuration.LeftBottomFinderPatternY, configuration.FinderPatternDiameter);
        }
        else
        {
            DrawFinderPatternRectangleStyle(image, configuration.LeftTopFinderPatternX,
                configuration.LeftTopFinderPatternY, configuration.FinderPatternDiameter * 2, configuration.Multiple);
            DrawFinderPatternRectangleStyle(image, configuration.RightTopFinderPatternX,
                configuration.RightTopFinderPatternY, configuration.FinderPatternDiameter * 2, configuration.Multiple);
            DrawFinderPatternRectangleStyle(image, configuration.LeftBottomFinderPatternX,
                configuration.LeftBottomFinderPatternY, configuration.FinderPatternDiameter * 2, configuration.Multiple);
        }
    }

    private void DrawFinderPatternCircleStyle(Image<TPixel> image, int x, int y, int circleDiameter)
    {
        var whiteCircleDiameter = 5 * circleDiameter / 7;
        var middleDotDiameter = 3 * circleDiameter / 7;

        image.Mutate(img =>
        {
            var circle = new EllipsePolygon(x + circleDiameter, y + circleDiameter, circleDiameter);
            img.Fill(_finderPatternBrush, circle);
            circle = new EllipsePolygon(x + circleDiameter, y + circleDiameter, whiteCircleDiameter);
            img.Fill(Color.White, circle);
            circle = new EllipsePolygon(x + circleDiameter, y + circleDiameter, middleDotDiameter);
            img.Fill(_finderPatternBrush, circle);
        });
    }

    private void DrawFinderPatternRectangleStyle(Image<TPixel> image, int x, int y, int size, int offset)
    {
        var whiteRectangleSize = 5 * size / 7;
        var centerRectangleSize = 3 * size / 7;

        image.Mutate(img =>
        {
            var circle = new RectangularPolygon(x, y, size, size);
            img.Fill(_finderPatternBrush, circle);
            circle = new RectangularPolygon(x + offset, y + offset, whiteRectangleSize, whiteRectangleSize);
            img.Fill(Color.White, circle);
            circle = new RectangularPolygon(x + 2*offset, y + 2*offset, centerRectangleSize, centerRectangleSize);
            img.Fill(_finderPatternBrush, circle);
        });
    }
}