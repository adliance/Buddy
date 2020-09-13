namespace Adliance.AspNetCore.Buddy.Pdf.Test
{
    public class MockedAdliancePdferSettings : IAdliancePdferSettings
    {
        public string PdfServerUrl => "https://adliance-pdf-on-linux.azurewebsites.net/html-to-pdf";
    }
}