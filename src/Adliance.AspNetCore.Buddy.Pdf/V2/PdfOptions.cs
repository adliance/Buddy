namespace Adliance.AspNetCore.Buddy.Pdf.V2
{
    public class PdfOptions
    {
        public string? HeaderHtml { get; set; } = null;
        public int? HeaderHeight { get; set; } = null;
        public string? FooterHtml { get; set; } = null;
        public int? FooterHeight { get; set; } = null;
    }
}
