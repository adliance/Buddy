using Adliance.AspNetCore.Buddy.Pdf.V1;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V1
{
    public class MockedPdferConfiguration : IPdferConfiguration
    {
        public string ServerUrl => "https://adliance-pdf-on-linux.azurewebsites.net/html-to-pdf";
    }
}