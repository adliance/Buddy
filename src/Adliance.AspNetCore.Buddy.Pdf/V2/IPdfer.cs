using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.V2
{
    public interface IPdfer
    {
        Task<byte[]> HtmlToPdf(string html, PdfOptions options);
    }
}
