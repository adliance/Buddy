using System.Threading.Tasks;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Test;

public class MailjetEmailerTest
{
    [Fact]
    public async Task CanSendEmailWithoutAttachments()
    {
        var mailer = new MailjetEmailer(new MockedMailjetConfiguration(), new MockedEmailConfiguration());

        await mailer.Send(
            "hannes@sachsenhofer.com",
            "Unit Test for MailJet (no attachments)",
            "This is the <b>HTML</b> body.",
            "This is the **Text** body.");
    }

    [Fact]
    public async Task CanSendEmailWithAttachments()
    {
        var mailer = new MailjetEmailer(new MockedMailjetConfiguration(), new MockedEmailConfiguration());

        await mailer.Send(
            "hannes@sachsenhofer.com",
            "Unit Test for MailJet (with attachments)",
            "This is the <b>HTML</b> body.<br /><br />And <a href='https://www.igevia.com'>this</a> is a link.",
            "This is the **Text** body.",
            new MockedEmailAttachment("textfile.txt", new byte[] { 1 }),
            new MockedEmailAttachment("musicfile.mp3", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
    }
}
