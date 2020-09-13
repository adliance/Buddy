using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Pdf
{
    public class PdfRendererResult : IEmailAttachment
    {
        public string Html { get; set; } = "";
        public string Filename { get; set; } = "";
        public byte[] Bytes { get; set; } = new byte[0];
    }
}
