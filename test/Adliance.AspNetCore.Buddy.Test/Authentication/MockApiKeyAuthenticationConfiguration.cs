using Adliance.AspNetCore.Buddy.Authentication;

namespace Adliance.AspNetCore.Buddy.Test.Authentication;

public class MockApiKeyAuthenticationConfiguration : IApiKeyAuthenticationConfiguration
{
    public string HttpHeaderKey => "X-Test-Key";
    public string QueryStringKey => "Test-Key";
    public bool AllowEmptyApiKey => false;
}
