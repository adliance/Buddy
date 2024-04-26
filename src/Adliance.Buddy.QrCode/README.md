# Adliance.Buddy.QrCode

The QR Code Buddy makes generating QR Codes with images and special colors easy.

It uses the [ImageSharp](https://sixlabors.com/products/imagesharp/) and the [ZXing.Net](https://github.com/micjahn/ZXing.Net/)
libraries internally, and can be used in device and cloud scenarios.

**Note:** make sure to buy a [license for ImageSharp](https://sixlabors.com/pricing/), when you meet their criteria.

![Example QR code](https://github.com/adliance/Buddy/blob/master/test/Adliance.Buddy.QrCode.Tests/GoldenImages/complex-adliance.png?raw=true)

## Using the library

Let's start with the most simple example:

```csharp
var qrCode = new QrCodeBuilder<Rgba32>("https://adliance.net")
    .Render();
qrCode.SaveAsPng("qr-adliance.png");
```

### Rounded

To create the same QR code, but with rounded markers and content dots just add one line:

```csharp
var qrCode = new QrCodeBuilder<Rgba32>("https://adliance.net")
    .Rounded()
    .Render();
qrCode.SaveAsPng("qr-adliance.png");
```

### Overlay image

Adding a logo image in the center is as simple as adding another line:

```csharp
var qrCode = new QrCodeBuilder<Rgba32>("https://adliance.net")
    .Rounded()
    .WithOverlayImage(Image.Load("logo.png"))
    .WithOverlayMargin(2) // optional margin around the image
    .Render();
qrCode.SaveAsPng("qr-adliance.png");
```

### Colors

The background, finder patterns and content dots are individually configurable with [ImageSharp Brushes](https://docs.sixlabors.com/api/ImageSharp.Drawing/SixLabors.ImageSharp.Drawing.Processing.Brushes.html):

```csharp
var finderBrush = Brushes.Solid(Color.Gold);
var backgroundBrush = Brushes.Solid(Color.Black);

var topCenter = new PointF(125, 0);
var bottomCenter = new PointF(125, 250);
var repetitionMode = GradientRepetitionMode.None;
// approximating the QR code margin by setting the color stops at 10% and 90%
var topColor = new ColorStop(0.1f, Color.Yellow);
var bottomColor = new ColorStop(0.9f, Color.LimeGreen);

var contentBrush = new LinearGradientBrush(topCenter, bottomCenter,
                repetitionMode, topColor, bottomColor);

var qrCode = new QrCodeBuilder<Rgba32>("https://adliance.net")
    .WithContentBrush(contentBrush)
    .WithFinderPatternBrush(finderBrush)
    .WithBackgroundBrush(backgroundBrush)
    .Render();
qrCode.SaveAsPng("qr-adliance.png");
```

### Transparent background

```csharp
var qrCode = new QrCodeBuilder<Rgba32>("https://adliance.net")
    .Transparent()
    .Render();
qrCode.SaveAsPng("qr-adliance-transparent.png");
```

### QR Code settings

Other information, like the error correction level and margins can also be set:

```csharp
var qrCode = new QrCodeBuilder<Rgba32>("https://adliance.net")
    .WithDimensions(500, 500)
    .WithMargin(3)
    .WithErrorCorrectionLevel(ErrorCorrectionLevel.H)
    .Render();
qrCode.SaveAsPng("qr-adliance.png");
```