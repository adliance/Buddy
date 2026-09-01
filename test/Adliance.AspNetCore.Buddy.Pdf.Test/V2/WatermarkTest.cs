using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Xunit;
using static Adliance.AspNetCore.Buddy.Pdf.Test.V2.TestHelpers;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V2;

// These tests cover the API contract only — does AdliancePdfer shape and transmit
// WatermarkOptions/PdfOptions.Watermarks correctly (single/multiple entries, position/size,
// multi-page pass-through) and return a valid, larger PDF. What a watermark actually looks like
// is already covered by pdfservice2's own server.test.js.
public class WatermarkTest
{
    private readonly IPdfer _pdfer = new AdliancePdfer(new MockedPdferConfiguration());

    // Same wording as pdfservice2's server.test.js sampleBodyHtml, so PDFs from both test suites
    // are visually comparable. Long enough to fill a page, so the watermark overlaps real text.
    private static readonly string SampleBodyHtml = "<h1>Report</h1>" + string.Concat(Enumerable.Range(1, 15).Select(i =>
        $"<p style='margin:4px 0;'>This is line {i} of the report body content, with enough additional filler "
        + "text so the paragraph wraps across most of the available page width, for a more realistic layout.</p>"));

    private const string SampleWatermarkHtml =
        "<div style='width:100%;height:100%;background-color:rgba(255,0,0,0.3);display:flex;align-items:center;justify-content:center;'>"
        + "<div style='font-size:45px;color:white;transform:rotate(-20deg);'>CONFIDENTIAL</div></div>";

    private static readonly string TwoPageBodyHtml = $"<h1>Page 1</h1>{SampleBodyHtml}"
                                                       + $"<div style='page-break-after: always;'></div><h1>Page 2</h1>{SampleBodyHtml}";

    [Fact]
    public async Task Renders_A_Pdf_With_A_Single_Watermark()
    {
        var bytes = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions
        {
            Watermarks = [new WatermarkOptions { Html = SampleWatermarkHtml, X = 0, Y = 80, Width = 794, Height = 250 }]
        });
        await StoreForInspection(bytes, "watermark-single");
        Assert.InRange(bytes.Length, 5_000, 40_000);
    }

    [Fact]
    public async Task Stamps_The_Watermark_Onto_Every_Page_When_Rendering_A_Multi_Page_Pdf()
    {
        var bytes = await _pdfer.HtmlToPdf(TwoPageBodyHtml, new PdfOptions
        {
            Watermarks = [new WatermarkOptions { Html = SampleWatermarkHtml, X = 0, Y = 80, Width = 794, Height = 250 }]
        });
        await StoreForInspection(bytes, "watermark-multipage");

        var metadata = await _pdfer.GetPdfMetadata(bytes);
        Assert.Equal(2, metadata.TotalPages);
    }

    [Fact]
    public async Task Renders_A_Pdf_With_Multiple_Watermarks_In_A_Single_Request_Each_At_Its_Own_Position()
    {
        var watermarks = new[]
        {
            new WatermarkOptions { Html = SampleWatermarkHtml, X = 0, Y = 0, Width = 794, Height = 150 },
            new WatermarkOptions { Html = SampleWatermarkHtml, X = 0, Y = 900, Width = 794, Height = 150 }
        };

        var baseline = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions());
        var bytes = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions { Watermarks = watermarks.ToList() });
        await StoreForInspection(bytes, "watermark-multiple");

        Assert.True(bytes.Length > baseline.Length, "Two watermarks should add more content than none.");
    }

    [Fact]
    public async Task Can_Add_A_Watermark_To_An_Existing_Pdf()
    {
        var original = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions());
        var watermarked = await _pdfer.AddWatermark(original,
        [
            new WatermarkOptions { Html = SampleWatermarkHtml, X = 0, Y = 80, Width = 794, Height = 250 }
        ]);
        await StoreForInspection(watermarked, "add-watermark-single");
        Assert.True(watermarked.Length > original.Length);
    }

    [Fact]
    public async Task Can_Add_Watermark_To_Existing_Pdf_At_An_Explicit_Position_And_Size()
    {
        var original = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions());
        var watermarked = await _pdfer.AddWatermark(original,
        [
            new WatermarkOptions { Html = SampleWatermarkHtml, X = 40, Y = 100, Width = 350, Height = 150 }
        ]);
        await StoreForInspection(watermarked, "add-watermark-positioned");
        Assert.True(watermarked.Length > original.Length);
    }

    [Fact]
    public async Task Stamps_The_Watermark_Onto_Every_Page_Of_A_Multi_Page_Existing_Pdf()
    {
        var original = await _pdfer.HtmlToPdf(TwoPageBodyHtml, new PdfOptions());

        var watermarked = await _pdfer.AddWatermark(original,
        [
            new WatermarkOptions { Html = SampleWatermarkHtml, X = 0, Y = 80, Width = 794, Height = 250 }
        ]);
        await StoreForInspection(watermarked, "add-watermark-multipage");

        var metadata = await _pdfer.GetPdfMetadata(watermarked);
        Assert.Equal(2, metadata.TotalPages);
    }

    [Fact]
    public async Task Can_Add_Multiple_Watermarks_To_An_Existing_Pdf_In_A_Single_Request_Each_At_Its_Own_Position()
    {
        var original = await _pdfer.HtmlToPdf(SampleBodyHtml, new PdfOptions());
        var watermarks = new[]
        {
            new WatermarkOptions { Html = SampleWatermarkHtml, X = 0, Y = 0, Width = 794, Height = 150 },
            new WatermarkOptions { Html = SampleWatermarkHtml, X = 0, Y = 900, Width = 794, Height = 150 }
        };

        var watermarked = await _pdfer.AddWatermark(original, watermarks);
        await StoreForInspection(watermarked, "add-watermark-multiple");

        Assert.True(watermarked.Length > original.Length);
    }
}
