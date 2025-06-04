using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adliance.AspNetCore.Buddy.Bulma.TagHelpers;

[HtmlTargetElement("tabs")]
[RestrictChildren("tab")]
public class TabsTagHelper : TagHelper
{
    public override void Init(TagHelperContext context)
    {
        context.Items["tabs"] = new List<TabTagHelper>();
        base.Init(context);
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        (await output.GetChildContentAsync()).GetContent(); // do not remove because otherwise the child tag helpers are not executed

        var tabs = (List<TabTagHelper>)context.Items["tabs"];

        var builder = new HtmlContentBuilder();
        builder.AppendHtml("<div class=\"tabs is-boxed\">");
        builder.AppendHtml("<ul>");

        var tabIndex = 0;
        foreach (var tab in tabs)
        {
            builder.AppendHtml($"<li class=\"{(tab.IsActive ? "is-active" : "")}\">");

            var url = tab.Url;
            var onclick = "";
            if (string.IsNullOrWhiteSpace(url))
            {
                url = "#";
                onclick = ""
                + "this.closest('.tabs').querySelector('li.is-active').classList.remove('is-active');"
                + "this.closest('li').classList.add('is-active');"
                + "this.closest('.tabs').parentElement.querySelectorAll('div.tab').forEach(function(e) { e.classList.add('is-hidden'); });"
                + $"this.closest('.tabs').parentElement.querySelector('div.tab:nth-child({(tabIndex + 2)})').classList.remove('is-hidden');"
                + "return false;";
            }

            builder.AppendHtml($"<a href=\"{url}\" onclick=\"{onclick}\">");
            if (!string.IsNullOrWhiteSpace(tab.IconClassName)) builder.AppendHtml($"<span class=\"icon is-small\"><i class=\"{tab.IconClassName}\"></i></span>");
            builder.AppendHtml("<span>");
            builder.AppendHtml(tab.Title);
            builder.AppendHtml("</span>");
            builder.AppendHtml("</a>");
            builder.AppendHtml("</li>");
            tabIndex++;
        }

        builder.AppendHtml("</ul>");
        builder.AppendHtml("</div>");

        output.Content.SetHtmlContent(builder);
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        var postBuilder = new HtmlContentBuilder();

        tabIndex = 0;
        foreach (var tab in tabs)
        {
            postBuilder.AppendHtml($"<div class=\"tab{(tab.IsActive ? "" : " is-hidden")}\">");
            postBuilder.AppendHtml(tab.RenderedContent);
            postBuilder.AppendHtml("</div>");
            tabIndex++;
        }

        output.PostContent.SetHtmlContent(postBuilder);
    }
}

[HtmlTargetElement("tab", ParentTag = "tabs")]
public class TabTagHelper : TagHelper
{
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public bool IsActive { get; set; }
    public string? IconClassName { get; set; }
    internal string? RenderedContent { get; set; }

    public override void Init(TagHelperContext context)
    {
        ((List<TabTagHelper>)context.Items["tabs"]).Add(this);
        base.Init(context);
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childContent = (await output.GetChildContentAsync()).GetContent();
        output.Content.SetHtmlContent(childContent);
        RenderedContent = childContent;
    }
}
