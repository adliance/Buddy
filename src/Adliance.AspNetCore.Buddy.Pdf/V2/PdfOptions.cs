namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public class PdfOptions
{
    /// <summary>
    /// The HTML for the PDF header as string. This property is ignored if used when creating a PDF from a template,
    /// as the header information is provided in <see cref="TemplateOptions.Template"/>.
    /// </summary>
    public string? HeaderHtml { get; set; }

    /// <summary>
    /// The height of the header in pixel (px). If a HeaderHtml is provided, the height must be set.
    /// This property is ignored if used when creating a PDF from a template,
    /// as the header information is provided in <see cref="HeaderTemplateOptions.Height"/>.
    /// </summary>
    public int? HeaderHeight { get; set; }

    /// <summary>
    /// The HTML for the PDF header as string. This property is ignored if used when creating a PDF from a template,
    /// as the footer information is provided in <see cref="TemplateOptions.Template"/>.
    /// </summary>
    public string? FooterHtml { get; set; }

    /// <summary>
    /// The height of the footer in pixel (px). If a FooterHtml is provided, the height must be set.
    /// This property is ignored if used when creating a PDF from a template,
    /// as the footer information is provided in <see cref="FooterTemplateOptions.Height"/>.
    /// </summary>
    public int? FooterHeight { get; set; }

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

public enum PdfSize
{
    A4,
    A5,
    A3,
    A2,
    A6,
    Letter
}
