# Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices

The Azure Communication Buddy makes sending E-Mail easy. Find more detailed information on the [Azure website](https://learn.microsoft.com/en-us/azure/communication-services/concepts/email/prepare-email-communication-resource).

## Setup library in an ASP.NET project

The `IBuddyServiceCollection` of the `Adliance.AspNetCore.Buddy.Abstractions` package offers the `AddBuddy` method, which provides `AddAzureCommunicationEmail` extensions to add the E-Mail services.

```c#
using Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices;

// ...

public void ConfigureServices(IServiceCollection services)
{
  //...
  services.AddBuddy()
    .AddAzureCommunicationEmail(Configuration.GetSection("Email"),
      Configuration.GetSection("AzureCommunicationEmail"))    
  //...
 }
```

### Configuration (appsettings.json)

Add a section in the configuration of your project and add following configuration:

```json
{
  "Email": {
    "SenderName": "unused (always taken from config in Azure)",
    "SenderAddress": "sender@example.com",
    "ReplyToAddress": "reply@example.com",
    "RedirectAllEmailsTo": "",
    "Disable": false
  },
  "AzureCommunicationEmail": {
    "Endpoint": "https://<your-communication-service-name>.<region>.communication.azure.com/",
    "AccessKey": "your-access-key",
    "UserEngagementTrackingDisabled": false
  }
}
```

Look up your API credentials in your Azure portal.

## Usage of library

This code sample shows the usage of the Azure Communication email client. Just call the `Send` method providing a recipient, subject and a body.
```c#
IEmailer emailer = new AzureCommunicationEmailer(azureCommunicationConfig, emailConfig);
await emailer.Send(recipientAddress,
            "Descriptive subject line",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body.");
```
