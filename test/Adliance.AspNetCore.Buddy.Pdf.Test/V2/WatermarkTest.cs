using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Xunit;
using static Adliance.AspNetCore.Buddy.Pdf.Test.V2.TestHelpers;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V2;

public class WatermarkTest
{
    private readonly IPdfer _pdfer = new AdliancePdfer(new MockedPdferConfiguration());

    // Same wording as pdfservice2's server.test.js sampleBodyHtml, so PDFs from both test suites
    // are visually comparable. Long enough to fill a page, so the watermark overlaps real text.
    private static readonly string SampleBodyHtml = "<h1>Report</h1>" + string.Concat(Enumerable.Range(1, 15).Select(i =>
        $"<p style='margin:4px 0;'>This is line {i} of the report body content, with enough additional filler "
        + "text so the paragraph wraps across most of the available page width, for a more realistic layout.</p>"));

    // backgroundAlpha (rgba() alpha) and opacityProperty (CSS opacity) are NOT interchangeable:
    // the former only fades the fill, the latter fades the whole element including the text.
    // Values need InvariantCulture — under en-AT etc. plain interpolation renders 0.3 as "0,3",
    // producing invalid CSS that silently fails to parse.
    private static string SampleWatermarkHtml(double backgroundAlpha = 1, double opacityProperty = 1)
    {
        var alpha = backgroundAlpha.ToString(CultureInfo.InvariantCulture);
        var opacity = opacityProperty.ToString(CultureInfo.InvariantCulture);
        return $"<div style='width:100%;height:100%;opacity:{opacity};background-color:rgba(255,0,0,{alpha});display:flex;align-items:center;justify-content:center;'>"
            + "<div style='font-size:45px;color:white;transform:rotate(-20deg);'>CONFIDENTIAL</div></div>";
    }

    private static readonly string TwoPageBodyHtml = $"<h1>Page 1</h1>{SampleBodyHtml}"
                                                       + $"<div style='page-break-after: always;'></div><h1>Page 2</h1>{SampleBodyHtml}";

    // A full HTML document, not just a fragment. Background is on a nested <div>, not <body> —
    // a <body> background is propagated by CSS to the page canvas and prints as an always-opaque
    // base layer regardless of alpha. height:100% is set on <html> so the nested div's own
    // height:100% resolves against a defined ancestor instead of collapsing.
    private static string SampleFullDocumentWatermarkHtml(double backgroundAlpha)
    {
        var alpha = backgroundAlpha.ToString(CultureInfo.InvariantCulture);
        return $"""
            <!DOCTYPE html>
            <html style="height:100%;" lang="en">
            <head><meta charset="UTF-8" /></head>
            <body style="margin:0;height:100%;">
            <div style="width:100%;height:100%;background-color:rgba(255,0,0,{alpha});display:flex;align-items:center;justify-content:center;">
              <div style="font-size:45px;color:white;transform:rotate(-20deg);">CONFIDENTIAL</div>
            </div>
            </body>
            </html>
            """;
    }

    [Fact]
    public async Task With_A_Fully_Opaque_Watermark_Background_Alpha_1_Should_Obscure_Content_Behind_It()
    {
        var bytes = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(backgroundAlpha: 1), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await StoreForInspection(bytes, "watermark-background-alpha-opaque");
        Assert.InRange(bytes.Length, 5_000, 40_000);
    }

    [Fact]
    public async Task With_A_Semi_Transparent_Watermark_Background_Alpha_0_3_Should_Let_Content_Behind_It_Remain_Visible()
    {
        var bytes = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(backgroundAlpha: 0.3), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await StoreForInspection(bytes, "watermark-background-alpha-transparent");
        Assert.InRange(bytes.Length, 5_000, 40_000);
    }

    [Fact]
    public async Task With_A_Fully_Opaque_Watermark_Css_Opacity_Property_1_Should_Obscure_Content_Behind_It()
    {
        var bytes = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(opacityProperty: 1), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await StoreForInspection(bytes, "watermark-opacity-property-opaque");
        Assert.InRange(bytes.Length, 5_000, 40_000);
    }

    [Fact]
    public async Task With_A_Semi_Transparent_Watermark_Css_Opacity_Property_0_3_Should_Let_Content_Behind_It_Remain_Visible()
    {
        var bytes = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(opacityProperty: 0.3), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await StoreForInspection(bytes, "watermark-opacity-property-transparent");
        Assert.InRange(bytes.Length, 5_000, 40_000);
    }

    // The byte-range assertions above only prove each variant renders successfully — not that
    // opacity/alpha actually had an effect. If the service silently ignored it, both PDFs would
    // still land in the same range and those tests would keep passing regardless.
    [Fact]
    public async Task Opaque_And_Transparent_Watermark_Background_Alpha_Should_Produce_Different_Pdfs()
    {
        var opaqueTask = _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(backgroundAlpha: 1), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        var transparentTask = _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(backgroundAlpha: 0.3), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await Task.WhenAll(opaqueTask, transparentTask);

        Assert.NotEqual(await opaqueTask, await transparentTask);
    }

    [Fact]
    public async Task Opaque_And_Transparent_Watermark_Css_Opacity_Property_Should_Produce_Different_Pdfs()
    {
        var opaqueTask = _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(opacityProperty: 1), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        var transparentTask = _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(opacityProperty: 0.3), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await Task.WhenAll(opaqueTask, transparentTask);

        Assert.NotEqual(await opaqueTask, await transparentTask);
    }

    [Fact]
    public async Task Can_Add_Fully_Opaque_Watermark_Background_Alpha_1_To_Existing_Pdf_Obscuring_Content_Behind_It()
    {
        var original = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions());
        var watermarked = await _pdfer.AddWatermark(original, new WatermarkOptions
        {
            Html = SampleWatermarkHtml(backgroundAlpha: 1), X = 0, Y = 80, Width = 794, Height = 250
        });
        await StoreForInspection(watermarked, "add-watermark-background-alpha-opaque");
        Assert.True(watermarked.Length > original.Length);
    }

    [Fact]
    public async Task Can_Add_Semi_Transparent_Watermark_Background_Alpha_0_3_To_Existing_Pdf_So_Content_Behind_It_Remains_Visible()
    {
        var original = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions());
        var watermarked = await _pdfer.AddWatermark(original, new WatermarkOptions
        {
            Html = SampleWatermarkHtml(backgroundAlpha: 0.3), X = 0, Y = 80, Width = 794, Height = 250
        });
        await StoreForInspection(watermarked, "add-watermark-background-alpha-transparent");
        Assert.True(watermarked.Length > original.Length);
    }

    // Like header/footer, the watermark is rendered per page, so it supports the same
    // .current-page/.total-pages substitution and .only-on-first-page/.not-on-first-page classes.
    private const string SampleCurrentPageWatermarkHtml =
        "<div style='width:100%;height:100%;display:flex;flex-direction:column;align-items:center;justify-content:center;'>"
        + "<div style='font-size:35px;color:red;'>Page <span class='current-page'></span> of <span class='total-pages'></span></div>"
        + "<div class='only-on-first-page' style='font-size:30px;color:blue;'>DRAFT</div>"
        + "<div class='not-on-first-page' style='font-size:30px;color:green;'>FINAL</div>"
        + "</div>";

    [Fact]
    public async Task Substitutes_Current_Page_And_Total_Pages_And_Honors_Page_Conditional_Classes_In_The_Watermark_When_Rendering()
    {
        var bytes = await _pdfer.HtmlToPdf(TwoPageBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleCurrentPageWatermarkHtml, X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await StoreForInspection(bytes, "watermark-page-conditional");

        var metadata = await _pdfer.GetPdfMetadata(bytes);
        Assert.Equal(2, metadata.TotalPages);
    }

    [Fact]
    public async Task Substitutes_Current_Page_And_Total_Pages_And_Honors_Page_Conditional_Classes_In_The_Watermark_When_Adding_To_An_Existing_Pdf()
    {
        var original = await _pdfer.HtmlToPdf(TwoPageBodyHtml, new PdfOptions());

        var watermarked = await _pdfer.AddWatermark(original, new WatermarkOptions
        {
            Html = SampleCurrentPageWatermarkHtml, X = 0, Y = 80, Width = 794, Height = 250
        });
        await StoreForInspection(watermarked, "add-watermark-page-conditional");

        var metadata = await _pdfer.GetPdfMetadata(watermarked);
        Assert.Equal(2, metadata.TotalPages);
    }

    [Fact]
    public async Task Stamps_The_Watermark_Onto_Every_Page_When_Rendering_A_Multi_Page_Pdf()
    {
        var bytes = await _pdfer.HtmlToPdf(TwoPageBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleWatermarkHtml(backgroundAlpha: 0.3), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await StoreForInspection(bytes, "watermark-multipage");

        var metadata = await _pdfer.GetPdfMetadata(bytes);
        Assert.Equal(2, metadata.TotalPages);
    }

    [Fact]
    public async Task Stamps_The_Watermark_Onto_Every_Page_Of_A_Multi_Page_Existing_Pdf()
    {
        var original = await _pdfer.HtmlToPdf(TwoPageBodyHtml, new PdfOptions());

        var watermarked = await _pdfer.AddWatermark(original, new WatermarkOptions
        {
            Html = SampleWatermarkHtml(backgroundAlpha: 0.3), X = 0, Y = 80, Width = 794, Height = 250
        });
        await StoreForInspection(watermarked, "add-watermark-multipage");

        var metadata = await _pdfer.GetPdfMetadata(watermarked);
        Assert.Equal(2, metadata.TotalPages);
    }

    [Fact]
    public async Task Renders_Correctly_When_Watermark_Html_Is_A_Full_Document()
    {
        var opaqueTask = _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleFullDocumentWatermarkHtml(1), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        var transparentTask = _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermark = new WatermarkOptions { Html = SampleFullDocumentWatermarkHtml(0.3), X = 0, Y = 80, Width = 794, Height = 250 }
        });
        await Task.WhenAll(opaqueTask, transparentTask);

        var opaque = await opaqueTask;
        var transparent = await transparentTask;
        await StoreForInspection(opaque, "watermark-fulldocument-opaque");
        await StoreForInspection(transparent, "watermark-fulldocument-transparent");

        // the only difference between the two requests is the alpha baked into the watermark's
        // own CSS — the resulting PDFs must not be byte-identical
        Assert.NotEqual(opaque, transparent);
    }

    [Fact]
    public async Task Can_Add_Watermark_To_Existing_Pdf_At_An_Explicit_Position_And_Size()
    {
        var original = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions());
        var watermarked = await _pdfer.AddWatermark(original, new WatermarkOptions
        {
            Html = SampleWatermarkHtml(backgroundAlpha: 0.3), X = 40, Y = 100, Width = 350, Height = 150
        });
        await StoreForInspection(watermarked, "add-watermark-positioned");
        Assert.True(watermarked.Length > original.Length);
    }
}
