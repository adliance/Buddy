namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public class WatermarkOptions
{
    /// <summary>
    /// The HTML for the watermark as string. Transparency is not a separate option — control it
    /// directly in this HTML/CSS (e.g. <c>opacity: 0.3</c> on the root element, or <c>rgba()</c>/
    /// <c>hsla()</c> colors), exactly like header/footer/body HTML already work.
    /// </summary>
    public required string Html { get; set; }

    /// <summary>
    /// The horizontal position (px) of the watermark, measured from the page's top-left corner. Defaults to 0.
    /// </summary>
    public int? X { get; set; }

    /// <summary>
    /// The vertical position (px) of the watermark, measured from the page's top-left corner. Defaults to 0.
    /// </summary>
    public int? Y { get; set; }

    /// <summary>
    /// The width (px) of the box the watermark HTML is rendered into. Defaults to the full page width.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// The height (px) of the box the watermark HTML is rendered into. Defaults to the full page height.
    /// </summary>
    public int? Height { get; set; }
}
