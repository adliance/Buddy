using Xunit;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

public class SmsRendererTest
{
    [Fact]
    public async Task SendsTheRenderedTemplateUsingTheDefaultDirectory()
    {
        var templater = new MockTemplater();
        var smser = new MockSmser();
        var renderer = new SmsRenderer(smser, templater);

        await renderer.RenderAndSendAsync("+43123456789", "SomeTemplate", new { Name = "World" });

        var sent = Assert.Single(smser.SentMessages);
        Assert.Equal("+43123456789", sent.Recipient);
        Assert.Equal("SmsTemplates/SomeTemplate: { Name = World }", sent.Text);

        var call = Assert.Single(templater.Calls);
        Assert.Equal("SmsTemplates", call.DirectoryName);
        Assert.Equal("SomeTemplate", call.TemplateName);
    }

    [Fact]
    public async Task SendsTheRenderedTemplateUsingACustomDirectory()
    {
        var templater = new MockTemplater();
        var smser = new MockSmser();
        var renderer = new SmsRenderer(smser, templater);

        await renderer.RenderAndSendAsync("+43123456789", "CustomDirectory", "SomeTemplate", new { });

        var call = Assert.Single(templater.Calls);
        Assert.Equal("CustomDirectory", call.DirectoryName);
        Assert.Equal("SomeTemplate", call.TemplateName);
    }

    [Fact]
    public async Task TrimsTheRenderedText()
    {
        var templater = new MockTemplater { RenderOverride = (_, _, _) => "  some text \n" };
        var smser = new MockSmser();
        var renderer = new SmsRenderer(smser, templater);

        await renderer.RenderAndSendAsync("+43123456789", "SomeTemplate", new { });

        var sent = Assert.Single(smser.SentMessages);
        Assert.Equal("some text", sent.Text);
    }

    [Fact]
    public async Task SendsAnEmptyMessageIfTheTemplateIsMissing()
    {
        var templater = new MockTemplater();
        templater.TemplateNamesToThrowFor.Add("SomeTemplate");
        var smser = new MockSmser();
        var renderer = new SmsRenderer(smser, templater);

        await renderer.RenderAndSendAsync("+43123456789", "SomeTemplate", new { });

        var sent = Assert.Single(smser.SentMessages);
        Assert.Equal("", sent.Text);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ThrowsIfRecipientAddressIsMissing(string recipientAddress)
    {
        var renderer = new SmsRenderer(new MockSmser(), new MockTemplater());

        await Assert.ThrowsAsync<ArgumentException>(() => renderer.RenderAndSendAsync(recipientAddress, "SomeTemplate", new { }));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ThrowsIfRecipientAddressIsMissingWithCustomDirectory(string recipientAddress)
    {
        var renderer = new SmsRenderer(new MockSmser(), new MockTemplater());

        await Assert.ThrowsAsync<ArgumentException>(() => renderer.RenderAndSendAsync(recipientAddress, "CustomDirectory", "SomeTemplate", new { }));
    }

    [Fact]
    public async Task PropagatesExceptionsFromTheSmsGateway()
    {
        var smser = new MockSmser { ExceptionToThrow = new InvalidOperationException("gateway failed") };
        var renderer = new SmsRenderer(smser, new MockTemplater());

        await Assert.ThrowsAsync<InvalidOperationException>(() => renderer.RenderAndSendAsync("+43123456789", "SomeTemplate", new { }));
    }
}
