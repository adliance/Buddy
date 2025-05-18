namespace Adliance.AspNetCore.Buddy.Sms.Twilio;

/// <inheritdoc cref="ITwilioSmsConfiguration"/>
public class TwilioSmsConfiguration : ITwilioSmsConfiguration
{
    /// <inheritdoc cref="ITwilioSmsConfiguration.AccountSid"/>
    public string AccountSid { get; set; } = "";

    /// <inheritdoc cref="ITwilioSmsConfiguration.AuthToken"/>
    public string AuthToken { get; set; } = "";

    /// <inheritdoc cref="ITwilioSmsConfiguration.FromPhoneNumber"/>
    public string FromPhoneNumber { get; set; } = "";
}
