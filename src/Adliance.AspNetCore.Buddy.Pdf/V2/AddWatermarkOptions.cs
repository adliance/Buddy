namespace Adliance.AspNetCore.Buddy.Pdf.V2;

/// <summary>
/// Rendering options for <see cref="IPdfer.AddWatermark"/> — the same knobs <see cref="PdfOptions"/>
/// exposes for <see cref="IPdfer.HtmlToPdf"/>, so a watermark stamped onto an existing PDF can be
/// made to match a document rendered elsewhere (e.g. with a non-default <see cref="Scale"/>).
/// </summary>
public class AddWatermarkOptions
{
    /// <summary>
    /// The scaling (zoom) the browser engine should use when rendering each watermark.
    /// </summary>
    public double? Scale { get; set; }

    /// <summary>
    /// Whether or not background images/colors should be printed for each watermark.
    /// </summary>
    public bool? PrintBackground { get; set; }

    /// <summary>
    /// Whether or not each watermark's own render should include a PDF outline (metadata). Has no
    /// visible effect — a watermark's outline never survives being embedded onto the target page —
    /// accepted only for consistency with <see cref="PdfOptions.Outline"/>.
    /// </summary>
    public bool? Outline { get; set; }
}
