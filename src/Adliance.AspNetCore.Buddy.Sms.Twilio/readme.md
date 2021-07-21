# Adliance.AspNetCore.Buddy.Sms.Twilio

The Twilio Buddy makes sending SMS easy. Find more detailed information on the Twilio website https://www.twilio.com/docs/sms.

## Setup library in an ASP.NET project

The `IBuddyServiceCollection` of the `Adliance.AspNetCore.Buddy.Abstractions` package offers the `AddBuddy` method, which provides `AddTwilio` extensions to add the SMS services.

```c#
public void ConfigureServices(IServiceCollection services)
{
  //...
  services.AddBuddy()
    .AddTwilio(Configuration.GetSection("Twilio"))    
  //...
 }
```

### Configuration (appsettings.json)

Add a section in the configuration of your project and add following configuration:

```json
{
  "Twilio": {
    "AccountSid": "your-account-sid",
    "AuthToken": "your-auth-token",
    "FromPhoneNumber": "+1234567890"
  }
}
```

Look up your API credentials in your Twilio account.

## Usage of library

This code sample shows the usage of the Twilio SMS client. Just call the `SendAsync` method providing a recipient and a text.
```c#
ISMSer _smser = new TwilioSmser(twilioSmsConfiguration, logger);
await _smser.SendAsync("+0987654321", "This is a sample message!");
```
