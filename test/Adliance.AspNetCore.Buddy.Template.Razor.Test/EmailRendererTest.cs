using Adliance.AspNetCore.Buddy.Abstractions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

public class EmailRendererTest
{
    private static IEmailRecipient Recipient(string address) => new EmailRecipient
    {
        Name = address,
        EmailAddress = address
    };

    private static RenderableEmail SomeEmail(object? viewModel = null) => new(
        "someone@example.com",
        "SomeTemplate",
        viewModel ?? new { Name = "World" });

    [Fact]
    public async Task RendersCorrectly()
    {
        var templater = new MockTemplater();
        var renderer = new EmailRenderer(templater, new MockEmailer());

        var email = SomeEmail();
        var result = await renderer.Render(email);

        Assert.Equal("EmailTemplates/SomeTemplate.Subject: { Name = World }", result.Subject);
        Assert.Equal("EmailTemplates/SomeTemplate.Html: { Name = World }", result.HtmlBody);
        Assert.Equal("EmailTemplates/SomeTemplate.Text: { Name = World }", result.TextBody);
    }

    [Fact]
    public async Task PassesTemplateDirectoryAndViewModelToTemplater()
    {
        var templater = new MockTemplater();
        var renderer = new EmailRenderer(templater, new MockEmailer());

        var viewModel = new { Name = "Jane" };
        var email = new RenderableEmail("someone@example.com", "SomeDirectory", "MySubject", "MyHtml", "MyText", viewModel);
        await renderer.Render(email);

        Assert.Equal(3, templater.Calls.Count);
        Assert.All(templater.Calls, c => Assert.Equal("SomeDirectory", c.DirectoryName));
        Assert.All(templater.Calls, c => Assert.Same(viewModel, c.ViewModel));
        Assert.Contains(templater.Calls, c => c.TemplateName == "MySubject");
        Assert.Contains(templater.Calls, c => c.TemplateName == "MyHtml");
        Assert.Contains(templater.Calls, c => c.TemplateName == "MyText");
    }

    [Fact]
    public async Task TrimsRenderedSubjectHtmlAndText()
    {
        var templater = new MockTemplater
        {
            RenderOverride = (_, _, _) => "  some content \n"
        };
        var renderer = new EmailRenderer(templater, new MockEmailer());

        var result = await renderer.Render(SomeEmail());

        Assert.Equal("some content", result.Subject);
        Assert.Equal("some content", result.HtmlBody);
        Assert.Equal("some content", result.TextBody);
    }

    [Fact]
    public async Task RendersWithoutTextTemplateIfNotAvailable()
    {
        var templater = new MockTemplater();
        templater.TemplateNamesToThrowFor.Add("SomeTemplate.Text");
        var renderer = new EmailRenderer(templater, new MockEmailer());

        var result = await renderer.Render(SomeEmail());

        Assert.Equal("", result.TextBody);
        Assert.False(string.IsNullOrEmpty(result.Subject));
        Assert.False(string.IsNullOrEmpty(result.HtmlBody));
    }

    [Fact]
    public async Task ThrowsIfSubjectTemplateIsMissing()
    {
        var templater = new MockTemplater();
        templater.TemplateNamesToThrowFor.Add("SomeTemplate.Subject");
        var renderer = new EmailRenderer(templater, new MockEmailer());

        await Assert.ThrowsAsync<InvalidOperationException>(() => renderer.Render(SomeEmail()));
    }

    [Fact]
    public async Task ThrowsIfHtmlTemplateIsMissing()
    {
        var templater = new MockTemplater();
        templater.TemplateNamesToThrowFor.Add("SomeTemplate.Html");
        var renderer = new EmailRenderer(templater, new MockEmailer());

        await Assert.ThrowsAsync<InvalidOperationException>(() => renderer.Render(SomeEmail()));
    }

    [Fact]
    public async Task RenderResultCarriesRecipientsAndAttachments()
    {
        var templater = new MockTemplater();
        var renderer = new EmailRenderer(templater, new MockEmailer());

        var to = new[] { Recipient("to@example.com") };
        var cc = new[] { Recipient("cc@example.com") };
        var bcc = new[] { Recipient("bcc@example.com") };
        var attachment = new MockEmailAttachment("file.txt", [1, 2, 3]);
        var email = new RenderableEmail(to, cc, bcc, "SomeDirectory", "MySubject", "MyHtml", "MyText", new { }, attachment);

        var result = await renderer.Render(email);

        Assert.Same(to, result.To);
        Assert.Same(cc, result.Cc);
        Assert.Same(bcc, result.Bcc);
        Assert.Single(result.Attachments);
        Assert.Same(attachment, result.Attachments[0]);
        Assert.Null(result.Sender);
    }

    [Fact]
    public async Task RenderAndSendSendsTheRenderedEmail()
    {
        var templater = new MockTemplater();
        var mailer = new MockEmailer();
        var renderer = new EmailRenderer(templater, mailer);

        await renderer.RenderAndSend(SomeEmail());

        var sent = Assert.Single(mailer.SentEmails);
        Assert.Equal("EmailTemplates/SomeTemplate.Subject: { Name = World }", sent.Subject);
        Assert.Equal("EmailTemplates/SomeTemplate.Html: { Name = World }", sent.HtmlBody);
    }

    [Fact]
    public async Task RenderAndSendThrowsIfMailerThrows()
    {
        var templater = new MockTemplater();
        var mailer = new MockEmailer { ExceptionToThrow = new InvalidOperationException("mailer failed") };
        var renderer = new EmailRenderer(templater, mailer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => renderer.RenderAndSend(SomeEmail()));
    }

    [Fact]
    public async Task RenderAndSendWithCallbackSendsTheRenderedEmail()
    {
        var templater = new MockTemplater();
        var mailer = new MockEmailer();
        var renderer = new EmailRenderer(templater, mailer);

        var callbackCount = 0;
        Exception? reportedException = null;

        await renderer.RenderAndSend(ex =>
        {
            callbackCount++;
            reportedException = ex;
        }, SomeEmail());

        var sent = Assert.Single(mailer.SentEmails);
        Assert.Equal("EmailTemplates/SomeTemplate.Subject: { Name = World }", sent.Subject);
        Assert.Equal("EmailTemplates/SomeTemplate.Html: { Name = World }", sent.HtmlBody);
        Assert.Equal(1, callbackCount);
        Assert.Null(reportedException);
    }

    [Fact]
    public async Task RenderAndSendWithCallbackReportsExceptionWhenSendingFails()
    {
        var templater = new MockTemplater();
        var exception = new InvalidOperationException("mailer failed");
        var mailer = new MockEmailer { ExceptionToThrow = exception };
        var renderer = new EmailRenderer(templater, mailer);

        var callbackCount = 0;
        Exception? reportedException = null;

        await renderer.RenderAndSend(ex =>
        {
            callbackCount++;
            reportedException = ex;
        }, SomeEmail());

        Assert.Empty(mailer.SentEmails);
        Assert.Equal(1, callbackCount);
        Assert.Same(exception, reportedException);
    }

    [Fact]
    public async Task RenderAndSendWithCallbackThrowsIfRenderingFails()
    {
        var templater = new MockTemplater();
        templater.TemplateNamesToThrowFor.Add("SomeTemplate.Subject");
        var mailer = new MockEmailer();
        var renderer = new EmailRenderer(templater, mailer);

        var callbackCount = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() => renderer.RenderAndSend(_ => callbackCount++, SomeEmail()));

        Assert.Empty(mailer.SentEmails);
        Assert.Equal(0, callbackCount);
    }
}
