using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.V1
{
    public interface IPdfer
    {
        Task<byte[]> HtmlToPdf(string html, PdfOptions options);
    }
}
