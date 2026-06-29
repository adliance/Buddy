using Xunit;

namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices.Test;

public class AzureCommunicationEmailerTest
{
    private readonly string _testRecipientAddress =
        Utils.GetEnvironmentVariable("Adliance_Buddy_Tests__AzureCommunication_RecipientAddress");

    [Fact]
    public async Task CanSendEmailWithoutAttachments()
    {
        var mailer = new AzureCommunicationEmailer(new MockedAzureCommunicationConfiguration(), new MockedEmailConfiguration());

        await mailer.Send(
            _testRecipientAddress,
            "Unit Test for AzureCommunicationEmailer (no attachments)",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body.");
    }

    [Fact]
    public async Task ThrowsExceptionOnInvalidAccessKey()
    {
        var invalidConfig = new MockedAzureCommunicationConfiguration()
        {
            AccessKey = Convert.ToBase64String("invalid"u8.ToArray())
        };

        var mailer = new AzureCommunicationEmailer(invalidConfig, new MockedEmailConfiguration());

        var exp = await Assert.ThrowsAsync<Exception>(async () => await mailer.Send(
            _testRecipientAddress,
            "Unit Test for AzureCommunicationEmailer (no attachments)",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body."));

        Assert.Contains("Denied", exp.Message);
        Assert.Contains("Unauthorized", exp.Message);
    }

    [Fact]
    public async Task CanSendEmailWithAttachments()
    {
        var mailer = new AzureCommunicationEmailer(new MockedAzureCommunicationConfiguration(), new MockedEmailConfiguration());

        await mailer.Send(
            _testRecipientAddress,
            "Unit Test for AzureCommunicationEmailer (with attachments)",
            "This is the <b>HTML</b> body.<br /><br />And <a href='https://www.igevia.com'>this</a> is a link.",
            "This is the **Text** body.",
            new MockedEmailAttachment("textfile.txt", [1]),
            new MockedEmailAttachment("musicfile.mp3", [1, 2, 3, 4, 5, 6, 7, 8, 9]));
    }

    [Fact]
    public async Task CanSendEmailNonBlocking()
    {
        var mailer = new AzureCommunicationEmailer(new MockedAzureCommunicationConfiguration(), new MockedEmailConfiguration());
        var tcs = new TaskCompletionSource<Exception?>();

        mailer.SendNonBlocking(
            tcs.SetResult,
            _testRecipientAddress,
            "Unit Test for AzureCommunicationEmailer NonBlocking (no attachments)",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body.");

        Assert.Null(await tcs.Task);
    }

    [Fact]
    public async Task NonBlockingCallsCallbackWithExceptionOnInvalidAccessKey()
    {
        var invalidConfig = new MockedAzureCommunicationConfiguration()
        {
            AccessKey = Convert.ToBase64String("invalid"u8.ToArray())
        };

        var mailer = new AzureCommunicationEmailer(invalidConfig, new MockedEmailConfiguration());
        var tcs = new TaskCompletionSource<Exception?>();

        mailer.SendNonBlocking(
            tcs.SetResult,
            _testRecipientAddress,
            "Unit Test for AzureCommunicationEmailer NonBlocking (no attachments)",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body.");

        var result = await tcs.Task;
        Assert.NotNull(result);
        Assert.Contains("Denied", result.Message);
        Assert.Contains("Unauthorized", result.Message);
    }

    [Fact]
    public async Task CanSendEmailNonBlockingWithAttachments()
    {
        var mailer = new AzureCommunicationEmailer(new MockedAzureCommunicationConfiguration(), new MockedEmailConfiguration());
        var tcs = new TaskCompletionSource<Exception?>();

        mailer.SendNonBlocking(
            tcs.SetResult,
            _testRecipientAddress,
            "Unit Test for AzureCommunicationEmailer NonBlocking (with attachments)",
            "This is the <b>HTML</b> body.<br /><br />And <a href='https://www.igevia.com'>this</a> is a link.",
            "This is the **Text** body.",
            new MockedEmailAttachment("textfile.txt", [1]),
            new MockedEmailAttachment("musicfile.mp3", [1, 2, 3, 4, 5, 6, 7, 8, 9]));

        Assert.Null(await tcs.Task);
    }
}
