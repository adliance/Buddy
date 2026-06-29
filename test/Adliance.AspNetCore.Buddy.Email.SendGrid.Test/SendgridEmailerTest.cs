using System;
using System.Threading.Tasks;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid.Test;

public class SendgridEmailerTest
{
    [Fact(Skip = "No API key configured.")]
    public async Task CanSendEmailWithoutAttachments()
    {
        var mailer = new SendgridEmailer(new MockedSendgridConfiguration(), new MockedEmailConfiguration());

        await mailer.Send(
            "hannes@sachsenhofer.com",
            "Unit Test for SendGrid (no attachments)",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body.");
    }

    [Fact(Skip = "No API key configured.")]
    public async Task CanSendEmailWithAttachments()
    {
        var mailer = new SendgridEmailer(new MockedSendgridConfiguration(), new MockedEmailConfiguration());

        await mailer.Send(
            "hannes@sachsenhofer.com",
            "Unit Test for SendGrid (with attachments)",
            "This is the <b>HTML</b> body.<br /><br />And <a href='https://www.igevia.com'>this</a> is a link.",
            "This is the **Text** body.",
            new MockedEmailAttachment("textfile.txt", new byte[] { 1 }),
            new MockedEmailAttachment("musicfile.mp3", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
    }

    [Fact(Skip = "No API key configured.")]
    public async Task CanSendEmailNonBlocking()
    {
        var mailer = new SendgridEmailer(new MockedSendgridConfiguration(), new MockedEmailConfiguration());
        var tcs = new TaskCompletionSource<Exception?>();

        mailer.SendNonBlocking(
            tcs.SetResult,
            "hannes@sachsenhofer.com",
            "Unit Test for SendGrid NonBlocking (no attachments)",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body.");

        Assert.Null(await tcs.Task);
    }

    [Fact(Skip = "No API key configured.")]
    public async Task CanSendEmailNonBlockingWithAttachments()
    {
        var mailer = new SendgridEmailer(new MockedSendgridConfiguration(), new MockedEmailConfiguration());
        var tcs = new TaskCompletionSource<Exception?>();

        mailer.SendNonBlocking(
            tcs.SetResult,
            "hannes@sachsenhofer.com",
            "Unit Test for SendGrid NonBlocking (with attachments)",
            "This is the <b>HTML</b> body.<br /><br />And <a href='https://www.igevia.com'>this</a> is a link.",
            "This is the **Text** body.",
            new MockedEmailAttachment("textfile.txt", new byte[] { 1 }),
            new MockedEmailAttachment("musicfile.mp3", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

        Assert.Null(await tcs.Task);
    }
}
