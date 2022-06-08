using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adliance.AspNetCore.Buddy.Bulma.TagHelpers;

[HtmlTargetElement("field")]
public class FieldTagHelper : TagHelper
{
    private readonly IHtmlGenerator _htmlGenerator;

    public FieldTagHelper(IHtmlGenerator htmlGenerator)
    {
        _htmlGenerator = htmlGenerator;
    }

    [HtmlAttributeNotBound, ViewContext] public ViewContext ViewContext { get; set; } = null!;
    [HtmlAttributeName("asp-for")] public ModelExpression? For { get; set; } = null!;
    [HtmlAttributeName("label")] public string? Label { get; set; }
    [HtmlAttributeName("icon")] public string? Icon { get; set; }
    [HtmlAttributeName("help")] public string? Help { get; set; }
    [HtmlAttributeName("placeholder")] public string? Placeholder { get; set; }
    [HtmlAttributeName("items")] public IEnumerable<SelectListItem>? Items { get; set; }
    [HtmlAttributeName("multi-line")] public bool MultiLine { get; set; }
    [HtmlAttributeName("rows")] public int Rows { get; set; } = 6;
    [HtmlAttributeName("password")] public bool Password { get; set; }
    [HtmlAttributeName("readonly")] public bool Readonly { get; set; }
    [HtmlAttributeName("disabled")] public bool Disabled { get; set; }


    private bool IsCheckBox => Items == null && (For?.ModelExplorer.ModelType == typeof(bool) || For?.ModelExplorer.ModelType == typeof(bool?));
    private bool IsDateTime => Items == null && (For?.ModelExplorer.ModelType == typeof(DateTime) || For?.ModelExplorer.ModelType == typeof(DateTime?));
    private bool IsSelectList => !IsCheckBoxList && Items != null;
    private bool IsCheckBoxList => Items != null && For != null && typeof(IEnumerable).IsAssignableFrom(For.ModelExplorer.ModelType);
    private bool HasIcon => !string.IsNullOrWhiteSpace(Icon);

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childContent = (await output.GetChildContentAsync()).GetContent();

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "field");

        var builder = new HtmlContentBuilder();

        // ReSharper disable MustUseReturnValue
        AppendLabel(builder);
        builder.AppendHtml($"<div class=\"control is-expanded{(HasIcon ? " has-icons-left" : "")}\">");
        if (!string.IsNullOrWhiteSpace(childContent)) builder.AppendHtml(childContent);
        AppendSelect(builder);
        AppendCheckbox(builder);
        AppendCheckBoxList(builder);
        AppendTextbox(builder);
        AppendValidationSection(builder);
        AppendHelpSection(builder);
        builder.AppendHtml("</div>");
        // ReSharper restore MustUseReturnValue

        output.Content.SetHtmlContent(builder);
    }

    private void AppendLabel(IHtmlContentBuilder builder)
    {
        if (IsCheckBox) return;
        if (For?.ModelExplorer == null)
        {
            builder.AppendHtml($"<label class=\"label\">{Label}</label>");
            return;
        }

        var label = _htmlGenerator.GenerateLabel(ViewContext, For?.ModelExplorer, For?.Name, Label, new { @class = "label" });
        builder.AppendHtml(label);
    }

    private void AppendSelect(IHtmlContentBuilder builder)
    {
        if (!IsSelectList || For?.ModelExplorer == null) return;
        var select = _htmlGenerator.GenerateSelect(ViewContext, For?.ModelExplorer, null, For?.Name, Items, false, new { });
        builder.AppendHtml("<div class=\"select is-fullwidth\">");
        builder.AppendHtml(select);
        if (HasIcon) builder.AppendHtml($"<span class=\"icon is-left\"><i class=\"fad fa-{Icon}\"></i></span>");
        builder.AppendHtml("</div>");
    }

    private void AppendCheckbox(IHtmlContentBuilder builder)
    {
        if (!IsCheckBox || For?.ModelExplorer == null) return;
        var checkbox = _htmlGenerator.GenerateCheckBox(ViewContext, For?.ModelExplorer, For?.Name, For?.Model is true, new { });
        builder.AppendHtml("<label class=\"checkbox\">");
        builder.AppendHtml(checkbox);
        if (!string.IsNullOrWhiteSpace(Icon))
        {
            builder.AppendHtml("&nbsp;&nbsp;");
            builder.AppendHtml($"<i class=\"fad fa-{Icon}\"></i>");
        }

        builder.AppendHtml("&nbsp;&nbsp;");
        if (HasIcon) builder.AppendHtml($"<i class=\"fad {Icon}\"></i>");
        builder.AppendHtml(Label ?? "");
        builder.AppendHtml("</label>");
    }

    private void AppendCheckBoxList(IHtmlContentBuilder builder)
    {
        if (!IsCheckBoxList || For?.ModelExplorer == null || Items == null) return;

        IList<string> selectedItems = (For.ModelExplorer.Model as List<object>)?.Select(x => x.ToString() ?? "").ToList()
                                      ?? (For.ModelExplorer.Model as List<Guid>)?.Select(x => x.ToString()).ToList()
                                      ?? (For.ModelExplorer.Model as List<string>)?.ToList()
                                      ?? (For.ModelExplorer.Model as IEnumerable<string>)?.ToList()
                                      ?? (For.ModelExplorer.Model as IEnumerable<object>)?.Select(x => x.ToString() ?? "").ToList()
                                      ?? new List<string>();

        foreach (var item in Items)
        {
            builder.AppendHtml("<label class=\"checkbox\">");
            builder.AppendHtml($"<input type=\"checkbox\" value=\"{item.Value}\" name=\"{For.ModelExplorer.Metadata.Name}\" {(selectedItems.Any(x => x.Equals(item.Value, StringComparison.OrdinalIgnoreCase)) ? "checked" : "")} >");
            if (!string.IsNullOrWhiteSpace(Icon))
            {
                builder.AppendHtml("&nbsp;&nbsp;");
                builder.AppendHtml($"<i class=\"fad fa-{Icon}\"></i>");
            }

            builder.AppendHtml("&nbsp;&nbsp;");
            if (HasIcon) builder.AppendHtml($"<i class=\"fad {Icon}\"></i>");
            builder.AppendHtml(item.Text ?? "");
            builder.AppendHtml("</label>");
        }
    }

    private void AppendTextbox(IHtmlContentBuilder builder)
    {
        if (IsSelectList || IsCheckBox || IsCheckBoxList || For?.ModelExplorer == null) return;

        TagBuilder control;

        if (Password)
        {
            control = _htmlGenerator.GeneratePassword(ViewContext, For?.ModelExplorer, For?.Name, null, new
            {
                @class = "input",
                placeholder = Placeholder ?? ""
            });
        }
        else if (MultiLine)
        {
            control = _htmlGenerator.GenerateTextArea(ViewContext, For?.ModelExplorer, For?.Name, Rows, 0, new
            {
                @class = "textarea",
                placeholder = Placeholder ?? ""
            });
        }
        else if (IsDateTime)
        {
            control = _htmlGenerator.GenerateTextBox(ViewContext, For?.ModelExplorer, For?.Name, (For?.ModelExplorer.Model as DateTime?)?.ToString("yyyy-MM-dd"), null, new { type = "date", @class = "input", placeholder = Placeholder ?? "" });
        }
        else
        {
            control = _htmlGenerator.GenerateTextBox(ViewContext, For?.ModelExplorer, For?.Name, For?.ModelExplorer.Model, null, new
            {
                @class = "input",
                placeholder = Placeholder ?? ""
            });
        }

        if (Readonly) control.Attributes.Add("readonly", "readonly");
        if (Disabled) control.Attributes.Add("disabled", "disabled");
        builder.AppendHtml(control);

        if (HasIcon) builder.AppendHtml($"<span class=\"icon is-left\"><i class=\"fad fa-{Icon}\"></i></span>");
    }

    private void AppendValidationSection(IHtmlContentBuilder builder)
    {
        if (For == null) return;
        var validation = _htmlGenerator.GenerateValidationMessage(ViewContext, For.ModelExplorer, For.Name, null, "div", new { @class = "help is-danger" });
        builder.AppendHtml(validation);
    }

    private void AppendHelpSection(IHtmlContentBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Help)) return;
        builder.AppendHtml("<div class=\"help\">");
        builder.AppendHtml(Help);
        builder.AppendHtml("</div>");
    }
}