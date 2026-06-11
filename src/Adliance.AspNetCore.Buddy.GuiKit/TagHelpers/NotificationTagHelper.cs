using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adliance.AspNetCore.Buddy.GuiKit.TagHelpers;

[HtmlTargetElement(TagHelperConstants.TagPrefix + "notification")]
public class NotificationTagHelper : TagHelper
{
    public NotificationLevel Level { get; set; }
    public string? Icon { get; set; }
    public bool AddIconAutomatically { get; set; } = true;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var contentHtml = (await output.GetChildContentAsync()).GetContent();

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.AddClass("notification", HtmlEncoder.Default);
        output.AddClass(Level.ToString().ToLower(), HtmlEncoder.Default);

        if (AddIconAutomatically && string.IsNullOrWhiteSpace(Icon))
        {
            Icon = Level switch
            {
                NotificationLevel.Info => "fa-hexagon-exclamation",
                NotificationLevel.Warning => "fa-hexagon-exclamation",
                NotificationLevel.Error => "fa-hexagon-xmark",
                NotificationLevel.Success => "fa-hexagon-check",
                _ => ""
            };
        }

        if (!string.IsNullOrWhiteSpace(Icon))
        {
            contentHtml = $"""
                           <i class='fa {Icon}'></i>
                           <div>{contentHtml}</div>
                           """;
        }

        output.Content.SetHtmlContent(contentHtml);
    }

    public enum NotificationLevel
    {
        Info,
        Warning,
        Error,
        Success
    }
}
