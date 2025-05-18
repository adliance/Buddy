namespace Adliance.AspNetCore.Buddy.Email.SendGrid;

public interface ISendgridConfiguration
{
    /// <summary>
    /// The SendGrid API key.
    /// </summary>
    string SendgridSecret { get; }

    /// <summary>
    /// A category name for the sent messages. The name may not exceed 255 characters.
    /// </summary>
    string SendgridLabel { get; }
}
