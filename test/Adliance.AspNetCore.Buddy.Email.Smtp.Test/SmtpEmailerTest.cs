using System;
using System.Threading.Tasks;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Email.Smtp.Test;

public class MailjetEmailerTest
{
    [Fact(Skip = "No SMTP user configured.")]
    public async Task CanSendEmailWithoutAttachments()
    {
        var mailer = new SmtpEmailer(new MockedSmtpConfiguration(), new MockedEmailConfiguration());

        await mailer.Send(
            "hannes@sachsenhofer.com",
            "Unit Test for SmtpEmailer (no attachments)",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body.");
    }

    [Fact(Skip = "No SMTP user configured.")]
    public async Task CanSendEmailWithAttachments()
    {
        var mailer = new SmtpEmailer(new MockedSmtpConfiguration(), new MockedEmailConfiguration());

        await mailer.Send(
            "hannes@sachsenhofer.com",
            "Unit Test for SmtpEmailer (with attachments)",
            "This is the <b>HTML</b> body.<br /><br />And <a href='https://www.igevia.com'>this</a> is a link.",
            "This is the **Text** body.",
            new MockedEmailAttachment("textfile.txt", new byte[] { 1 }),
            new MockedEmailAttachment("musicfile.mp3", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
    }

    [Fact]
    public async Task CannotSendEmailWithoutAuthentication()
    {
        var smtpConfig = new MockedSmtpConfiguration { UserName = "" };
        var mailer = new SmtpEmailer(smtpConfig, new MockedEmailConfiguration());

        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await mailer.Send(
                "hannes@sachsenhofer.com",
                "Unit Test for SmtpEmailer (no attachments)",
                "This is the <b>HTML</b> body.",
                "This is the **Text** body.");
        });
    }
}