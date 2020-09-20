// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Adliance.AspNetCore.Buddy.Email.Mailjet
{
    public interface IMailjetConfiguration
    {
        string PublicApiKey { get; }
        string PrivateApiKey { get; }
        string Campaign { get; }
    }

    // ReSharper disable once UnusedType.Global
    public class DefaultMailjetConfiguration : IMailjetConfiguration
    {
        public string PublicApiKey { get; set; } = "";
        public string PrivateApiKey { get; set; } = "";
        public string Campaign { get; set; } = "";
    }
}