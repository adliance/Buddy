using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V2;

public class AdliancePdferTest
{
    private readonly IPdfer _pdfer = new AdliancePdfer(new MockedPdferConfiguration());

    [Fact]
    public async Task Can_Create_Simple_Pdf()
    {
        var bytes = await _pdfer.HtmlToPdf("This is <b>my</b> <u>HTML</u> test.", new PdfOptions());
        await StoreForInspection(bytes);
        Assert.InRange(bytes.Length, 5_000, 28_000);
    }

    [Fact]
    public async Task Can_Create_Pdf_With_Header_And_Footer()
    {
        var bytes = await _pdfer.HtmlToPdf("This is <b>my</b> <u>HTML</u> test.", new PdfOptions
        {
            FooterHeight = 40,
            HeaderHeight = 80,
            HeaderHtml = "<!DOCTYPE html>The <i>Header</i>",
            FooterHtml = "<!DOCTYPE html>The <s>Footer</s>"
        });
        await StoreForInspection(bytes);
        Assert.InRange(bytes.Length, 15_000, 70_000);
    }

    [Fact]
    public async Task Can_Create_Complex_Pdf()
    {
        string html;
        await using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Adliance.AspNetCore.Buddy.Pdf.Test.complex_html.html"))
        {
            Assert.NotNull(stream);
            using (var reader = new StreamReader(stream ?? throw new Exception("Stream should not be null at this point.")))
            {
                html = await reader.ReadToEndAsync();
            }
        }

        string footer;
        await using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Adliance.AspNetCore.Buddy.Pdf.Test.complex_footer.html"))
        {
            Assert.NotNull(stream);
            using (var reader = new StreamReader(stream ?? throw new Exception("Stream should not be null at this point.")))
            {
                footer = await reader.ReadToEndAsync();
            }
        }

        var bytes = await _pdfer.HtmlToPdf(html, new PdfOptions
        {
            HeaderHtml = null,
            HeaderHeight = null,
            FooterHeight = 100,
            FooterHtml = footer,
            Size = PdfSize.A4
        });
        await StoreForInspection(bytes);
        Assert.InRange(bytes.Length, 90_000, 750_000);
    }

    [Fact]
    public async Task Can_Set_Width_and_Height()
    {
        var bytes = await _pdfer.HtmlToPdf("This is <b>my</b> <u>HTML</u> test for width and height being set manually.", new PdfOptions
        {
            PaperWidth = 150,
            PaperHeight = 150
        });
        await StoreForInspection(bytes);
        Assert.InRange(bytes.Length, 14_000, 40_000);
    }

    [Fact]
    public async Task Can_Create_Simple_Pdf_From_Template()
    {
        const string template = "This is <b>my</b> <u>{{Text}}</u> test.";
        var bytes = await _pdfer.TemplateToPdf(
            body: new TemplateOptions { Template = template, Model = new { Text = "template" } },
            header: null,
            footer: null,
            options: new TemplatePdfOptions());
        await StoreForInspection(bytes);
        Assert.InRange(bytes.Length, 5_000, 29_000);
    }

    [Fact]
    public async Task Can_Create_Pdf_With_Header_And_Footer_From_Template()
    {
        const string template = "This is <b>my</b> <u>{{Text}}</u> test.";
        const string header = "<i>Custom {{HeaderText}}.</i>";
        const string footer = "This is a custom footer for {{FooterText}}.";
        var bytes = await _pdfer.TemplateToPdf(
            body: new TemplateOptions
            {
                Template = template,
                Model = new { Text = "template" }
            },
            header: new HeaderTemplateOptions
            {
                Template = header,
                Model = new { HeaderText = "Header" },
                Height = 80
            },
            footer: new FooterTemplateOptions
            {
                Template = footer,
                Model = new { FooterText = "Footer" },
                Height = 40
            },
            options: new TemplatePdfOptions());
        await StoreForInspection(bytes);
        Assert.InRange(bytes.Length, 15_000, 70_000);
    }

    [Fact]
    public async Task Can_Create_Pdf_With_Model_Transformation_From_Template()
    {
        const string template = "This is <b>my</b> <u>{{CustomText}}</u> test.";
        const string header = "<i>Custom {{HeaderText}}.</i>";
        const string footer = "This is a custom footer for {{FooterText}}.";
        const string javascript = "model.CustomText = model.Text; return model;";
        var bytes = await _pdfer.TemplateToPdf(
            body: new TemplateOptions
            {
                Template = template,
                Model = new { Text = "transformed model" },
                Javascript = javascript
            },
            header: new HeaderTemplateOptions
            {
                Template = header,
                Model = new { HeaderText = "Header" },
                Height = 80,
                Javascript = javascript
            },
            footer: new FooterTemplateOptions
            {
                Template = footer,
                Model = new { FooterText = "Footer" },
                Height = 40,
                Javascript = javascript
            },
            options: new TemplatePdfOptions());
        await StoreForInspection(bytes);
        Assert.InRange(bytes.Length, 15_000, 70_000);
    }

    private static async Task StoreForInspection(byte[] bytes)
    {
        var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        if (Directory.Exists(directory))
        {
            await File.WriteAllBytesAsync(Path.Combine(directory, Guid.NewGuid() + ".pdf"), bytes);
        }
    }
}
