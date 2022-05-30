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
}
