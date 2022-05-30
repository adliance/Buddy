using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Pdf.V2;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V2
{
    public class AdliancePdferTest
    {
        private readonly IPdfer _pdfer = new AdliancePdfer(new MockedPdferConfiguration());

        [Fact]
        public async Task Can_Create_Simple_Pdf()
        {
            var bytes = await _pdfer.HtmlToPdf("This is <b>my</b> <u>HTML</u> test.", new PdfOptions());
            await StoreForInspection(bytes);
            Assert.InRange(bytes.Length, 10_000, 28_000);
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
            Assert.InRange(bytes.Length, 25_000, 70_000);
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
            Assert.InRange(bytes.Length, 30_000, 40_000);
        }

        private async Task StoreForInspection(byte[] bytes)
        {
            var directory = @"C:\Users\Hannes\Downloads\";
            if (Directory.Exists(directory))
            {
                await File.WriteAllBytesAsync(Path.Combine(directory, Guid.NewGuid() + ".pdf"), bytes);
            }
        }
    }
}