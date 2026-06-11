using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adliance.AspNetCore.Buddy.GuiKit.TagHelpers;

[HtmlTargetElement(TagHelperConstants.TagPrefix + "panel")]
public class PanelTagHelper : TagHelper
{
    public string? Title { get; set; }

    /// <summary>
    /// Name of the FontAwesome icon, eg. "fa-trash".
    /// </summary>
    public string? Icon { get; set; }

    public string? HxGet { get; set; }
    public string HxTrigger { get; set; } = "load";
    public CollapsibleStat Collapsible { get; set; } = CollapsibleStat.Disabled;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();

        output.TagName = "article";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.RemoveAll("hx-get");
        output.Attributes.RemoveAll("hx-trigger");
        output.Attributes.RemoveAll("collapsible");

        var collapsibleButtonHtml = "";
        if (Collapsible != CollapsibleStat.Disabled)
        {
            output.AddClass("collapsible", HtmlEncoder.Default);
            collapsibleButtonHtml = """
                                <button type="button" class="collapsible-toggle"><i class="fa fa-angle-down"></i></button>
                                """;
        }

        if (Collapsible == CollapsibleStat.Closed)
        {
            output.AddClass("collapsed", HtmlEncoder.Default);
        }

        var iconHtml = "";
        if (!string.IsNullOrWhiteSpace(Icon))
        {
            iconHtml =  $"<i class=\"fa {Icon}\"></i>";
        }

        var titleHtml = "";
        if (!string.IsNullOrWhiteSpace(Title))
        {
            titleHtml = $"<h2>{collapsibleButtonHtml}{iconHtml}{Title}</h2>";
        }

        var contentHtml = content.GetContent();
        if (!string.IsNullOrWhiteSpace(HxGet))
        {
            contentHtml = $"""
                           <div hx-get="{HxGet}" hx-trigger="{HxTrigger}">
                             {LoadingTagHelper.FullHtml}
                           </div>
                           {contentHtml}
                           """;
        }

        output.Content.SetHtmlContent($"{titleHtml}{contentHtml}");
    }

    public enum CollapsibleStat
    {
        Disabled,
        Opened,
        Closed
    }
}
