using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public interface IPdfer
{
    Task<byte[]> HtmlToPdf(string html, PdfOptions options);

    //TODO: separate class for template/options?
    Task<byte[]> TemplateToPdf(string template, object model, string? js, string? headerTemplate, object? headerModel, string? headerJs, string? footerTemplate, object? footerModel, string? footerJs, PdfOptions options);
}
