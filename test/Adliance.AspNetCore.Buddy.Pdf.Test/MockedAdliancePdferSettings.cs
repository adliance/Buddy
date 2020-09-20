namespace Adliance.AspNetCore.Buddy.Pdf.Test
{
    public class MockedPdferConfiguration : IPdferConfiguration
    {
        public string ServerUrl => "https://adliance-pdf-on-linux.azurewebsites.net/html-to-pdf";
    }
}