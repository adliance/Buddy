using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adliance.AspNetCore.Buddy.Bulma.TagHelpers;

[HtmlTargetElement("card")]
public class CardTagHelper : TagHelper
{
    public string? Title { get; set; }
    public string CollapsibleIconClassName { get; set; } = "far fa-angle-down";
    public bool? CollapsibleStartOpen { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childContent = (await output.GetChildContentAsync()).GetContent();

        var builder = new HtmlContentBuilder();

        if (!string.IsNullOrWhiteSpace(Title) || CollapsibleStartOpen.HasValue)
        {
            builder.AppendHtml($""""
                <header class="card-header">
                <p class="card-header-title">
                {Title}
                </p>
                """");

            if (CollapsibleStartOpen.HasValue)
            {
                builder.AppendHtml($""""
                <button class="card-header-icon" onclick="this.closest('.card').querySelector('.card-content').classList.toggle('is-hidden'); const icon=this.closest('.card').querySelector('.icon i'); icon.style.transform = icon.style.transform ? '' : 'scaleY(-1)';">
                <span class="icon">
                <i class="{CollapsibleIconClassName}" style="{(CollapsibleStartOpen == false ? "transform:scaleY(-1)" : "")}"></i>
                </span>
                </button>
                """");
            }

            builder.AppendHtml(""""
                </header>
                """");
        }

        builder.AppendHtml($""""
            <div class="card-content{(CollapsibleStartOpen == false ? " is-hidden" : "")}">
            """");
        builder.AppendHtml(childContent);
        builder.AppendHtml(""""
            </div>
            """");

        output.Content.SetHtmlContent(builder);
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.AddClass("card", HtmlEncoder.Default);
    }
}