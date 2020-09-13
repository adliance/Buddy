using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf;

namespace Adliance.AspNetCore.Buddy.Template.Razor
{
    public interface IPdfRenderer
    {
        Task<PdfRendererResult> Render(string templateBaseName, object viewModel);
        Task<PdfRendererResult> Render(string templateBaseName, PdfOptions options, object viewModel);
        Task<PdfRendererResult> Render(string templateDirectoryName, string pdfTemplateName, string filenameTemplateName, string headerTemplateName, string footerTemplateName, PdfOptions options, object viewModel);
    }
}