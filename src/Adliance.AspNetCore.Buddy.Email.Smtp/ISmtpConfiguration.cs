// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Adliance.AspNetCore.Buddy.Email.Smtp
{
    public interface ISmtpConfiguration
    {
        string Host { get; }
        int Port { get; }
        string? UserName { get; }
        string? Password { get; }
    }

    // ReSharper disable once UnusedType.Global
    public class DefaultSmtpConfiguration : ISmtpConfiguration
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 25;
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}