using Adliance.AspNetCore.Buddy.Pdf.V1;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V1
{
    public class MockedPdferConfiguration : IPdferConfiguration
    {
        public string ServerUrl => "https://pdf1.adliance.dev/html-to-pdf";
    }
}