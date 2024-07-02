// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Adliance.AspNetCore.Buddy.Email.Smtp
{
    public interface ISmtpConfiguration
    {
        /// <summary>
        /// The host name to connect to.
        /// </summary>
        string Host { get; }

        /// <summary>
        /// The port to connect to.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// The user name.
        /// </summary>
        string? UserName { get; }

        /// <summary>
        /// The password.
        /// </summary>
        string? Password { get; }
    }

    // ReSharper disable once UnusedType.Global
    public class DefaultSmtpConfiguration : ISmtpConfiguration
    {
        /// <inheritdoc />
        public string Host { get; set; } = "";

        /// <inheritdoc />
        /// <remarks>
        /// The default port is 25.
        /// </remarks>
        public int Port { get; set; } = 25;

        /// <inheritdoc />
        public string? UserName { get; set; }

        /// <inheritdoc />
        public string? Password { get; set; }
    }
}
