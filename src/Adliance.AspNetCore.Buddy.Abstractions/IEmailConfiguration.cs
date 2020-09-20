// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Adliance.AspNetCore.Buddy.Abstractions
{
    public interface IEmailConfiguration
    {
        /// <summary>
        /// The name of the sender.
        /// </summary>
        string SenderName { get; }

        /// <summary>
        /// The email address of the sender .
        /// </summary>
        string SenderAddress { get; }

        /// <summary>
        /// The "reply to" address (can be different from the sender address.
        /// </summary>
        string ReplyToAddress { get; }
    }

    // ReSharper disable once UnusedType.Global
    public class DefaultEmailConfiguration : IEmailConfiguration
    {
        public string SenderName { get; set; } = "";
        public string SenderAddress { get; set; } = "";
        public string ReplyToAddress { get; set; } = "";
    }
}