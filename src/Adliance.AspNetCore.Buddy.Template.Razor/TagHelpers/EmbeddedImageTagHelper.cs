using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Adliance.AspNetCore.Buddy.Template.Razor.TagHelpers
{
    /// <summary>
    /// <see cref="ITagHelper"/> implementation targeting &lt;eimg&gt; elements.
    /// </summary>
    [HtmlTargetElement("eimg", Attributes = SrcAttributeName)]
    public class EmbeddedImageTagHelper : UrlResolutionTagHelper
    {
        private const string SrcAttributeName = "src";

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// Source of the image.
        /// </summary>
        /// <remarks>
        /// Passed through to the generated HTML in all cases.
        /// </remarks>
        [HtmlAttributeName(SrcAttributeName)]
        public string? Source { get; set; }

        /// <summary>
        /// Creates a new <see cref="EmbeddedImageTagHelper"/>.
        /// </summary>
        /// <param name="htmlEncoder">The <see cref="HtmlEncoder"/> to use.</param>
        /// <param name="urlHelperFactory">The <see cref="IUrlHelperFactory"/>.</param>
        public EmbeddedImageTagHelper(
            HtmlEncoder htmlEncoder,
            IUrlHelperFactory urlHelperFactory)
            : base(urlHelperFactory, htmlEncoder)
        {
        }

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
                if (Source != null)
                {
                    if (TryResolveUrl(Source, out string? resolvedUrl))
                    {
                        if (resolvedUrl != null)
                        {
                            var base64 = GetBase64StringForImage(BuildImagePath(resolvedUrl));
                            output.Attributes.SetAttribute(SrcAttributeName,
                                $"data:{GetDataPartForImage(resolvedUrl)};base64,{base64}");
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;
        }

        public static string BuildImagePath(string url) => $"wwwroot{url}";

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