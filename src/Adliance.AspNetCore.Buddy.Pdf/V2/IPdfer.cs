using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public interface IPdfer
{
    Task<byte[]> HtmlToPdf(string html, PdfOptions options);

    Task<PdfMetadata> GetPdfMetadata(byte[] pdfBytes);

    Task<byte[]> TemplateToPdf(string template, object model, TemplateOptions options);

    Task<byte[]> AddWatermark(byte[] pdf, IEnumerable<WatermarkOptions> watermarks);
}
