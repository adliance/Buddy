namespace Adliance.AspNetCore.Buddy.Email.Mailjet
{
    public interface IMailjetConfiguration
    {
        string PublicApiKey { get; }
        string PrivateApiKey { get; }
        string Campaign { get; }
    }
}
