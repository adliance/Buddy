using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adliance.AspNetCore.Buddy.GuiKit.TagHelpers;

[HtmlTargetElement(TagHelperConstants.TagPrefix + "title")]
public class TitleTagHelper : TagHelper
{
    public required string Text { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        var contentHtml = content.GetContent().Trim();

        if (string.IsNullOrEmpty(contentHtml))
        {
            output.TagName = "h1";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(Text.Trim());
            return;
        }

        contentHtml = $"<p>{contentHtml}</p>";
        output.TagName = "hgroup";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Content.SetHtmlContent($"""
                                       <h1>{Text.Trim()}</h1>
                                       {contentHtml}
                                       """);
    }
}
