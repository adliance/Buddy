using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Adliance.AspNetCore.Buddy.Sms.Twilio;

/// <summary>
/// The Twilio SMS client.
/// </summary>
public class TwilioSmser : ISmser
{
    #region Constructor

    private readonly ITwilioSmsConfiguration _configuration;
    private readonly ILogger<TwilioSmser> _logger;

    public TwilioSmser(ITwilioSmsConfiguration configuration, ILogger<TwilioSmser> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    #endregion Constructor

    /// <summary>Initialize base client with username and password from the <see cref="ITwilioSmsConfiguration"/>.</summary>
    private void Initialize()
    {
        TwilioClient.Init(_configuration.AccountSid, _configuration.AuthToken);
    }

    /// <summary>
    /// Send a message from the account used in <see cref="ITwilioSmsConfiguration"/> to make the request.
    /// </summary>
    /// <param name="recipient">The destination phone number.</param>
    /// <param name="text">The text of the message you want to send. Can be up to 1,600 characters in length.</param>
    /// <returns>A task.</returns>
    public async Task SendAsync(string recipient, string text)
    {
        // ReSharper disable InconsistentLogPropertyNaming
        _logger.LogDebug("Send SMS to {recipient} with {text}", recipient, text);

        Initialize();

        var message = await MessageResource.CreateAsync(
            body: text,
            from: new PhoneNumber(_configuration.FromPhoneNumber),
            to: new PhoneNumber(recipient)
        );

        _logger.LogInformation("SMS sent: {message.Sid}", message.Sid);
    }
}
