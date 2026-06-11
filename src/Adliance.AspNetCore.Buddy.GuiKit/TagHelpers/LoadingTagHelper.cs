using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adliance.AspNetCore.Buddy.GuiKit.TagHelpers;

[HtmlTargetElement(TagHelperConstants.TagPrefix + "loading")]
public class LoadingTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Content.SetHtmlContent(FullHtml);
    }

    public const string FullHtml = """
                                   <div class="loading"></div>
                                   """;
}
