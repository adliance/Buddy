using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Pdf;

public class PdfRendererResult : IEmailAttachment
{
    public string Html { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public byte[] Bytes { get; set; } = [];
}
