using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf;
using Adliance.AspNetCore.Buddy.Pdf.V2;

namespace Adliance.AspNetCore.Buddy.Template.Razor.V2;

/// <summary>
/// The contract for a PDF renderer.
/// </summary>
public interface IPdfRenderer
{
    /// <summary>
    /// Renders a razor template provided in the "PdfTemplates" directory and converts it to a PDF.
    /// </summary>
    /// <param name="templateBaseName">The template's base name.</param>
    /// <param name="viewModel">The view model data for the razor template.</param>
    /// <returns>The rendered PDF as a <see cref="PdfRendererResult"/> object.</returns>
    Task<PdfRendererResult> Render(string templateBaseName, object viewModel);

    /// <summary>
    /// Renders a razor template provided in the "PdfTemplates" directory and converts it to a PDF.
    /// </summary>
    /// <param name="templateBaseName">The template's base name.</param>
    /// <param name="options">The PDF rendering options.</param>
    /// <param name="viewModel">The view model data for the razor template.</param>
    /// <returns>The rendered PDF as a <see cref="PdfRendererResult"/> object.</returns>
    Task<PdfRendererResult> Render(string templateBaseName, PdfOptions options, object viewModel);

    /// <summary>
    /// Renders several razor templates provided in the templateDirectoryName directory and converts it to a PDF.
    /// </summary>
    /// <param name="templateDirectoryName">The directory the templates are located in.</param>
    /// <param name="pdfTemplateName">The template's name of the PDF content.</param>
    /// <param name="filenameTemplateName">The template's name for the filename of the PDF.</param>
    /// <param name="headerTemplateName">The template's name for the PDF header.</param>
    /// <param name="footerTemplateName">The template's name for the PDF footer.</param>
    /// <param name="viewModel">The view model data for the razor template.</param>
    /// <param name="options">The PDF rendering options.</param>
    /// <returns>The rendered PDF as a <see cref="PdfRendererResult"/> object.</returns>
    Task<PdfRendererResult> Render(string templateDirectoryName, string pdfTemplateName,
        string filenameTemplateName, string headerTemplateName, string footerTemplateName, PdfOptions options,
        object viewModel);
}
