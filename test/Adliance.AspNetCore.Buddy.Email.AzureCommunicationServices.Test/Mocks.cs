using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices.Test;

// Environment variables for local testing can be set with [Solution personal layer](https://www.jetbrains.com/help/rider/Sharing_Configuration_Options.html#solution-personal-layer)
// settings for [Test Runner](https://www.jetbrains.com/help/rider/Reference__Options__Tools__Unit_Testing__Test_Runner.html#environment-variables)
public class MockedAzureCommunicationConfiguration : IAzureCommunicationConfiguration
{
    public string Endpoint => Utils.GetEnvironmentVariable("Adliance_Buddy_Tests__AzureCommunication_Endpoint");
    public string AccessKey { get; init; } = Utils.GetEnvironmentVariable("Adliance_Buddy_Tests__AzureCommunication_AccessKey");

    public bool UserEngagementTrackingDisabled => true;
}

public class MockedEmailConfiguration : IEmailConfiguration
{
    /// <summary>
    /// Azure Communication Services SDK does not support setting a sender name.
    /// It always uses the display name configured in the communication service
    /// domain -> MailFrom address.
    /// </summary>
    public string SenderName => "not used";
    public string SenderAddress => "DoNotReply@adliance.dev";
    public string ReplyToAddress => "hannes.sachsenhofer@adliance.net";
    public string RedirectAllEmailsTo => "";
    public bool Disable => false;
}

public class MockedEmailAttachment : IEmailAttachment
{
    public MockedEmailAttachment(string fileName, byte[] bytes)
    {
        Filename = fileName;
        Bytes = bytes;
    }

    public string Filename { get; set; }
    public byte[] Bytes { get; set; }
}
