using System.Diagnostics;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Template.Razor.V2
{
    /// <summary>
    /// Renders a razor template as PDF.
    /// </summary>
    public class PdfRenderer : IPdfRenderer
    {
        private readonly ITemplater _templater;
        private readonly IPdfer _pdfer;
        private readonly ILogger<PdfRenderer> _logger;

        public PdfRenderer(ITemplater templater, IPdfer pdfer, ILogger<PdfRenderer> logger)
        {
            _templater = templater;
            _pdfer = pdfer;
            _logger = logger;
        }
        
        /// <inheritdoc />
        public async Task<PdfRendererResult> Render(string templateBaseName, object viewModel)
        {
            return await Render(templateBaseName, null, viewModel);
        }
        
        /// <inheritdoc />
        public async Task<PdfRendererResult> Render(string templateBaseName, PdfOptions? options, object viewModel)
        {
            return await Render("PdfTemplates", $"{templateBaseName}", $"{templateBaseName}.Filename", $"{templateBaseName}.Header", $"{templateBaseName}.Footer", options, viewModel);
        }
        
        /// <inheritdoc />
        public async Task<PdfRendererResult> Render(string templateDirectoryName, string pdfTemplateName, string filenameTemplateName, string headerTemplateName, string footerTemplateName, PdfOptions? options, object viewModel)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            options ??= new PdfOptions(); // fall back to default options

            if (options.HeaderHtml == null && !string.IsNullOrWhiteSpace(headerTemplateName))
            {
                try
                {
                    options.HeaderHtml = (await _templater.Render(templateDirectoryName, headerTemplateName, viewModel)).Trim();
                    _logger.LogTrace($"Using header from template '{headerTemplateName}'.");
                }
                catch
                {
                    // do nothing here. Happens usually when template does not exist, and we don't want anything to happen in this case
                }
            }

            if (options.FooterHtml == null && !string.IsNullOrWhiteSpace(footerTemplateName))
            {
                try
                {
                    options.FooterHtml = (await _templater.Render(templateDirectoryName, footerTemplateName, viewModel)).Trim();
                    _logger.LogTrace($"Using footer from template '{footerTemplateName}'.");
                }
                catch
                {
                    // do nothing here. Happens usually when template does not exist, and we don't want anything to happen in this case
                }
            }

            var result = new PdfRendererResult
            {
                Html = (await _templater.Render(templateDirectoryName, pdfTemplateName, viewModel)).Trim(),
                Filename = (await _templater.Render(templateDirectoryName, filenameTemplateName, viewModel)).Trim(),
            };

            stopwatch.Stop();
            _logger.LogTrace($"Rendering of HTML templates took {stopwatch.ElapsedMilliseconds}ms.");

            stopwatch.Restart();
            result.Bytes = await _pdfer.HtmlToPdf(result.Html, options);
            _logger.LogTrace($"Conversion to PDF took {stopwatch.ElapsedMilliseconds}ms, the result is {result.Bytes.Length:N0} bytes.");

            return result;
        }
    }
}
