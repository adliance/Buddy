namespace Adliance.AspNetCore.Buddy.Abstractions
{
    public interface IEmailConfiguration
    {
        /// <summary>
        /// The name of the sender.
        /// </summary>
        string EmailSenderName { get; }

        /// <summary>
        /// The email address of the sender .
        /// </summary>
        string EmailSenderAddress { get; }
        
        /// <summary>
        /// The "reply to" address (can be different from the sender address.
        /// </summary>
        string EmailReplyToAddress { get; }
    }
}
