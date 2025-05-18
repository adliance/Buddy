namespace Adliance.AspNetCore.Buddy.Sms.Twilio;

public interface ITwilioSmsConfiguration
{
    /// <summary>
    /// The Account SID of your Twilio account.
    /// </summary>
    string AccountSid { get; }

    /// <summary>
    /// The Auth Token of your Twilio account. 
    /// </summary>
    string AuthToken { get; }

    /// <summary>
    /// The phone number that initiated the sent message.
    /// </summary>
    string FromPhoneNumber { get; }
}
