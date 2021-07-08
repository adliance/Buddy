namespace Adliance.AspNetCore.Buddy.Pdf.V2
{
    public class PdfOptions
    {
        /// <summary>
        /// The HTML for the PDF header as string.
        /// </summary>
        public string? HeaderHtml { get; set; } = null;
        
        /// <summary>
        /// The height of the header in pixel (px). If a HeaderHtml is provided, the height must be set.
        /// </summary>
        public int? HeaderHeight { get; set; } = null;
        
        /// <summary>
        /// The HTML for the PDF header as string.
        /// </summary>
        public string? FooterHtml { get; set; } = null;
        
        /// <summary>
        /// The height of the footer in pixel (px). If a FooterHtml is provided, the height must be set.
        /// </summary>
        public int? FooterHeight { get; set; } = null;
    }
}
