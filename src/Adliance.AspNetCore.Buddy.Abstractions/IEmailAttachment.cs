namespace Adliance.AspNetCore.Buddy.Abstractions
{
    /// <summary>
    /// Specifies the contract for an attachment of an email.
    /// </summary>
    public interface IEmailAttachment
    {
        /// <summary>
        /// The filename of the email attachment.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// The binary content of the email attachment. 
        /// </summary>
        byte[] Bytes { get; }
    }
}
