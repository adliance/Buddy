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

/// <summary>
/// A simple-to-use builder to render fancy QR codes.
/// </summary>
/// <param name="content">The content encoded in the QR code</param>
/// <typeparam name="TPixel"><see cref="SixLabors.ImageSharp.Image{TPixel}"/></typeparam>
public class QrCodeBuilder<TPixel>(string content)
    where TPixel : unmanaged, IPixel<TPixel>
{
    private int _width = 250;
    private int _height = 250;
    private int _margin = 4;

    private ErrorCorrectionLevel _errorCorrectionLevel = ErrorCorrectionLevel.L;

    private bool _roundedFinderPattern = false;
    private bool _roundedContentDots = false;

    private Brush _backgroundBrush = Brushes.Solid(Color.White);
    private Brush _finderPatternBrush = Brushes.Solid(Color.Black);
    private Brush _contentBrush = Brushes.Solid(Color.Black);

    private Image? _overlayImage;
    private int _overlayMargin = 0;

    /// <summary>
    /// Sets the dimensions of the generated QR code image.
    /// </summary>
    /// <param name="width">In pixels</param>
    /// <param name="height">In Pixels</param>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    /// <exception cref="ArgumentException">If one of the parameters is below 0</exception>
    public QrCodeBuilder<TPixel> WithDimensions(int width, int height)
    {
        if (width < 0 || height < 0) throw new ArgumentException("Height and width must be greater than 0.");

        _width = width;
        _height = height;

        return this;
    }

    /// <summary>
    /// Sets the error correction level of the QR code.
    /// Default: low.
    /// </summary>
    /// <param name="level">The error correction level</param>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithErrorCorrectionLevel(ErrorCorrectionLevel level)
    {
        _errorCorrectionLevel = level;
        return this;
    }

    /// <summary>
    /// Set the brush to paint the background. Default: solid white.
    /// </summary>
    /// <param name="brush">The ImageSharp brush</param>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithBackgroundBrush(Brush brush)
    {
        _backgroundBrush = brush;
        return this;
    }

    /// <summary>
    /// Sets the background to be transparent.
    /// </summary>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> Transparent()
    {
        _backgroundBrush = Brushes.Solid(Color.Transparent);
        return this;
    }

    /// <summary>
    /// Sets the brush to paint the finder patterns in the corners.
    /// Default: solid black.
    /// </summary>
    /// <param name="brush">The ImageSharp brush</param>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithFinderPatternBrush(Brush brush)
    {
        _finderPatternBrush = brush;
        return this;
    }

    /// <summary>
    /// Sets the brush to paint the data content.
    /// Default: solid black.
    /// </summary>
    /// <param name="brush">The ImageSharp brush</param>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithContentBrush(Brush brush)
    {
        _contentBrush = brush;
        return this;
    }

    /// <summary>
    /// Sets the finder patterns and content to be rendered rounded.
    /// </summary>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> Rounded()
    {
        WithRoundedFinderPattern();
        return WithRoundedContentDots();
    }

    /// <summary>
    /// Sets the finder patterns in the corners to be rendered rounded.
    /// </summary>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithRoundedFinderPattern()
    {
        _roundedFinderPattern = true;
        return this;
    }

    /// <summary>
    /// Sets the content dots to be rendered rounded.
    /// </summary>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithRoundedContentDots()
    {
        _roundedContentDots = true;
        return this;
    }

    /// <summary>
    /// Sets the margin around the QR code.
    /// </summary>
    /// <param name="numberOfDataPoints">The margin measured in the number of content dots.</param>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithMargin(int numberOfDataPoints)
    {
        _margin = numberOfDataPoints;
        return this;
    }

    /// <summary>
    /// Sets the image to be rendered over the center of the QR code.
    /// </summary>
    /// <param name="image">An ImageSharp image</param>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithOverlayImage(Image image)
    {
        _overlayImage = image;
        if (_errorCorrectionLevel.ordinal() <= 1)
        {
            _errorCorrectionLevel = ErrorCorrectionLevel.Q;
        }

        return this;
    }

    /// <summary>
    /// Sets a margin for the overlay image.
    /// Used in combination with <see cref="WithOverlayImage"/>.
    /// </summary>
    /// <param name="numberOfDataPoints">The margin measured in the number of content dots.</param>
    /// <returns><see cref="QrCodeBuilder{TPixel}"/> for further calls</returns>
    public QrCodeBuilder<TPixel> WithOverlayMargin(int numberOfDataPoints)
    {
        _overlayMargin = numberOfDataPoints;
        _errorCorrectionLevel = ErrorCorrectionLevel.H;
        return this;
    }
    
    /// <summary>
    /// Renders the final QR code image.
    /// </summary>
    /// <returns>the resulting QR code image</returns>
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
            Width = _width,
            Margin = _margin
        };

        var image = new Image<TPixel>(_width, _height);

        image.Mutate(x => x.Fill(_backgroundBrush));

        RenderContent(renderConfig, image);
        RenderFinderPattern(renderConfig, image);
        RenderOverlayImage(renderConfig, image, code);

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
                configuration.LeftTopFinderPatternY, configuration.FinderPatternOutputSize, configuration.Multiple);
            DrawFinderPatternRectangleStyle(image, configuration.RightTopFinderPatternX,
                configuration.RightTopFinderPatternY, configuration.FinderPatternOutputSize, configuration.Multiple);
            DrawFinderPatternRectangleStyle(image, configuration.LeftBottomFinderPatternX,
                configuration.LeftBottomFinderPatternY, configuration.FinderPatternOutputSize,
                configuration.Multiple);
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
            circle = new RectangularPolygon(x + 2 * offset, y + 2 * offset, centerRectangleSize, centerRectangleSize);
            img.Fill(_finderPatternBrush, circle);
        });
    }

    private void RenderOverlayImage(RenderConfiguration configuration, Image<TPixel> image, QRCode code)
    {
        if (_overlayImage == null) return;

        var cutoutSize = configuration.GetCutoutSize(code, _overlayMargin);
        var overlayImageSize = configuration.GetOverlayImageSize(code);
        _overlayImage.Mutate(x => x.Resize(overlayImageSize));
        var cutoutLocation = new Point((_width - cutoutSize.Width) / 2, (_height - cutoutSize.Height) / 2);
        var overlayImageLocation =
            new Point((_width - overlayImageSize.Width) / 2, (_height - overlayImageSize.Height) / 2);
        var cutout = new Rectangle(cutoutLocation, cutoutSize);
        image.Mutate(x =>
        {
            x.Clear(_backgroundBrush, cutout);
            x.DrawImage(_overlayImage, overlayImageLocation, 1f);
        });
    }
}