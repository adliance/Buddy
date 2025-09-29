using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V1;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V1;

public class AdliancePdferTest
{
    private readonly IPdfer _pdfer = new AdliancePdfer(new MockedPdferConfiguration());
    private const string LocalDebugDirectory = "/Users/hannes/Downloads";

    [Fact]
    public async Task Can_Create_Simple_Pdf()
    {
        var bytes = await _pdfer.HtmlToPdf("This is <b>my</b> <u>HTML</u> test.", new PdfOptions());
        var localDebugDirectory = new DirectoryInfo(LocalDebugDirectory);
        if (localDebugDirectory.Exists) await File.WriteAllBytesAsync(Path.Combine(localDebugDirectory.FullName, Guid.NewGuid() + ".pdf"), bytes);

        Assert.InRange(bytes.Length, 7_000, 14_000);
    }

    [Fact]
    public async Task Can_Create_Pdf_With_Header_And_Footer()
    {
        var bytes = await _pdfer.HtmlToPdf("This is <b>my</b> <u>HTML</u> test.", new PdfOptions
        {
            MarginTop = 50,
            MarginBottom = 50,
            HeaderSpacing = 10,
            FooterSpacing = 10,
            HeaderHtml = "<!DOCTYPE html>The <i>Header</i>",
            FooterHtml = "<!DOCTYPE html>The <s>Footer</s>"
        });

        var localDebugDirectory = new DirectoryInfo(LocalDebugDirectory);
        if (localDebugDirectory.Exists) await File.WriteAllBytesAsync(Path.Combine(localDebugDirectory.FullName, Guid.NewGuid() + ".pdf"), bytes);
        Assert.InRange(bytes.Length, 11_000, 20_000);
    }

    [Theory]
    [InlineData(PdfOrientation.Portrait, PdfSize.A2, 3_100_000, 4_000_000)]
    [InlineData(PdfOrientation.Portrait, PdfSize.A3, 3_600_000, 3_800_000)]
    [InlineData(PdfOrientation.Portrait, PdfSize.A4, 3_700_000, 4_000_000)]
    [InlineData(PdfOrientation.Portrait, PdfSize.A5, 3_100_000, 3_300_000)]
    [InlineData(PdfOrientation.Portrait, PdfSize.A6, 3_600_000, 4_100_000)]
    [InlineData(PdfOrientation.Portrait, PdfSize.Letter, 3_000_000, 4_000_000)]
    [InlineData(PdfOrientation.Landscape, PdfSize.A2, 3_600_000, 4_000_000)]
    [InlineData(PdfOrientation.Landscape, PdfSize.A3, 3_700_000, 4_000_000)]
    [InlineData(PdfOrientation.Landscape, PdfSize.A4, 3_700_000, 4_000_000)]
    [InlineData(PdfOrientation.Landscape, PdfSize.A5, 4_000_000, 4_200_000)]
    [InlineData(PdfOrientation.Landscape, PdfSize.A6, 3_600_000, 4_000_000)]
    [InlineData(PdfOrientation.Landscape, PdfSize.Letter, 3_100_000, 4_000_000)]
    public async Task Can_Create_Complex_Pdf(PdfOrientation orientation, PdfSize size, int lowerExpectedSize, int higherExpectedSize)
    {
        string html;
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Adliance.AspNetCore.Buddy.Pdf.Test.complex_html.html"))
        {
            Assert.NotNull(stream);
            using (var reader = new StreamReader(stream ?? throw new Exception("Stream should not be null at this point.")))
            {
                html = reader.ReadToEnd();
            }
        }

        string footer;
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Adliance.AspNetCore.Buddy.Pdf.Test.complex_footer.html"))
        {
            Assert.NotNull(stream);
            using (var reader = new StreamReader(stream ?? throw new Exception("Stream should not be null at this point.")))
            {
                footer = await reader.ReadToEndAsync();
            }
        }

        var bytes = await _pdfer.HtmlToPdf(html, new PdfOptions
        {
            MarginTop = 15,
            MarginBottom = 33,
            MarginLeft = 0,
            MarginRight = 0,
            FooterHtml = footer,
            Orientation = orientation,
            Size = size
        });

        var localDebugDirectory = new DirectoryInfo(LocalDebugDirectory);
        if (localDebugDirectory.Exists) await File.WriteAllBytesAsync(Path.Combine(localDebugDirectory.FullName, Guid.NewGuid() + ".pdf"), bytes);
        Assert.InRange(bytes.Length, lowerExpectedSize, higherExpectedSize);
    }
}
