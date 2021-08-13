using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.IO;

namespace Adliance.AspNetCore.Buddy.Template.Razor.TagHelpers
{
    /// <summary>
    /// <see cref="ITagHelper"/> implementation targeting &lt;eimg&gt; elements.
    /// </summary>
    [HtmlTargetElement("eimg", Attributes = SrcAttributeName)]
    public class EmbeddedImageTagHelper : TagHelper
    {
        private const string SrcAttributeName = "src";

        /// <summary>
        /// Source of the image.
        /// </summary>
        /// <remarks>
        /// Passed through to the generated HTML in all cases.
        /// </remarks>
        [HtmlAttributeName(SrcAttributeName)] public string? Source { get; set; }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            try
            {
                var base64 = GetBase64StringForImage(Source!.Replace("~", "wwwroot"));
                output.Attributes.SetAttribute(SrcAttributeName, $"data:{GetDataPartForImage(Source)};base64,{base64}");
            }
            catch
            {
                // ignored
            }

            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;
        }

        public static string GetBase64StringForImage(string imgPath)
        {
            var imageBytes = File.ReadAllBytes(imgPath);
            return Convert.ToBase64String(imageBytes);
        }

        public static string GetDataPartForImage(string imgPath)
        {
            return Path.GetExtension(imgPath).Equals(".svg", StringComparison.InvariantCultureIgnoreCase)
                ? "image/svg+xml"
                : "image/gif";
        }
    }
}
