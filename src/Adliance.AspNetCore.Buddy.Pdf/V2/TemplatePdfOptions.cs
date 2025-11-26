namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public class TemplatePdfOptions
{
    /// <summary>
    /// Specifies the paper size: e.g. A4, Letter, etc. (default A4)
    /// </summary>
    public PdfSize? Size { get; set; } = PdfSize.A4;

    /// <summary>
    /// Specifies the paper width (px). If set, overrules <see cref="Size"/>
    /// </summary>
    public int? PaperWidth { get; set; }

    /// <summary>
    /// Specifies the paper height (px). If set, overrules <see cref="Size"/>
    /// </summary>
    public int? PaperHeight { get; set; }

    /// <summary>
    /// Whether or not background images should be printed.
    /// </summary>
    public bool? PrintBackground { get; set; }

    /// <summary>
    /// The scaling (zoom) that the browser engine should use
    /// </summary>
    public double? Scale { get; set; }
}
