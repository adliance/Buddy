using Adliance.AspNetCore.Buddy.Pdf.V2;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V2;

public class MockedPdferConfiguration : IPdferConfiguration
{
    public string ServerUrl => "https://pdf2.adliance.dev";
}