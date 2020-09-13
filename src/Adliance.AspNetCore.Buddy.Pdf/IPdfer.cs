using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf
{
    public interface IPdfer
    {
        Task<byte[]> HtmlToPdf(string html, PdfOptions options);
    }
}
