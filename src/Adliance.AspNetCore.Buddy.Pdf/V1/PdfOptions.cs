namespace Adliance.AspNetCore.Buddy.Pdf.V1;

public class PdfOptions
{
    public string? HeaderHtml { get; set; }
    public string? FooterHtml { get; set; }
    public int HeaderSpacing { get; set; }
    public int FooterSpacing { get; set; }

    public int MarginTop { get; set; } = 15;
    public int MarginBottom { get; set; } = 15;
    public int MarginLeft { get; set; } = 10;
    public int MarginRight { get; set; } = 10;

    /// <summary>
    /// Specifies the paper size: e.g. A4, Letter, etc. (default A4)
    /// </summary>
    public PdfSize Size { get; set; } = PdfSize.A4;

    /// <summary>
    /// Specifies the paper orientation to Landscape or Portrait (default Portrait).
    /// </summary>
    public PdfOrientation Orientation { get; set; } = PdfOrientation.Portrait;

    public bool UseCompression { get; set; } = true;
    public int Dpi { get; set; } = 600;
    public int ImageDpi { get; set; } = 600;
    public int ImageQuality { get; set; } = 100;

    public bool UseSmartShrinking { get; set; } = true;
    public bool EnableJavaScript { get; set; } = true;
    public bool LoadImages { get; set; } = true;
    public bool PrintBackground { get; set; } = true;
    public bool PrintMediaType { get; set; }
    public bool EnablePlugins { get; set; }
}

public enum PdfSize
{
    A4,
    A5,
    A3,
    A2,
    A6,
    Letter
}

public enum PdfOrientation
{
    Portrait,
    Landscape
}
