namespace Adliance.AspNetCore.Buddy.Pdf.V2;

/// <summary>
/// A watermark whose content is rendered as a Handlebars template/model/js triple (like the
/// body and header/footer of <see cref="AdliancePdfer.TemplateToPdf"/>), instead of being used
/// as literal HTML. <see cref="WatermarkOptions.Html"/> holds the template string here, exactly
/// like <see cref="PdfOptions.HeaderHtml"/>/<see cref="PdfOptions.FooterHtml"/> hold a template
/// rather than literal HTML when used via <see cref="AdliancePdfer.TemplateToPdf"/>.
/// Only usable via <see cref="PdfOptions.Watermarks"/> on a <see cref="TemplateOptions"/> —
/// passing one to <see cref="AdliancePdfer.HtmlToPdf"/> or <see cref="AdliancePdfer.AddWatermark"/>
/// throws, since those endpoints have no templating support.
/// </summary>
public class TemplateWatermarkOptions : WatermarkOptions
{
    /// <summary>
    /// The model passed to the watermark's Handlebars template.
    /// </summary>
    public object? Model { get; set; }

    /// <summary>
    /// Optional JavaScript to transform <see cref="Model"/> before rendering. If omitted, the model is used as-is.
    /// </summary>
    public string? JavaScript { get; set; }
}
