using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Adliance.AspNetCore.Buddy.Bulma.TagHelpers;

[HtmlTargetElement("field")]
public class FieldTagHelper : TagHelper
{
    private readonly IHtmlGenerator _htmlGenerator;

    // Cf. https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.Core/src/ModelBinding/FormValueHelper.cs#L6
    private const string CultureInvariantFieldName = "__Invariant";

    public FieldTagHelper(IHtmlGenerator htmlGenerator)
    {
        _htmlGenerator = htmlGenerator;
    }

    [HtmlAttributeNotBound, ViewContext] public ViewContext ViewContext { get; set; } = null!;
    [HtmlAttributeName("asp-for")] public ModelExpression? For { get; set; } = null!;
    [HtmlAttributeName("label")] public string? Label { get; set; }
    [HtmlAttributeName("append")] public string? Append { get; set; }
    [HtmlAttributeName("prepend")] public string? Prepend { get; set; }
    [HtmlAttributeName("icon")] public string? Icon { get; set; }
    [HtmlAttributeName("help")] public string? Help { get; set; }
    [HtmlAttributeName("warning")] public string? Warning { get; set; }
    [HtmlAttributeName("is-warning")] public bool IsWarning { get; set; }
    [HtmlAttributeName("placeholder")] public string? Placeholder { get; set; }
    [HtmlAttributeName("items")] public IEnumerable<SelectListItem>? Items { get; set; }
    [HtmlAttributeName("multi-line")] public bool MultiLine { get; set; }
    [HtmlAttributeName("file-upload")] public bool FileUpload { get; set; }
    [HtmlAttributeName("file-upload-accept")] public string? FileUploadAcceptFilter { get; set; }
    [HtmlAttributeName("multiple-file-upload")] public bool MultipleFileUpload { get; set; }
    [HtmlAttributeName("multiple-file-upload-files-label")]
    public string? MultipleFileUploadFilesLabel { get; set; } = "Files";
    [HtmlAttributeName("checkboxes")] public bool Checkboxes { get; set; }
    [HtmlAttributeName("rows")] public int Rows { get; set; } = 6;
    [HtmlAttributeName("password")] public bool Password { get; set; }
    [HtmlAttributeName("number")] public bool Number { get; set; }
    [HtmlAttributeName("format")] public string? Format { get; set; }
    [HtmlAttributeName("required")] public bool Required { get; set; }
    [HtmlAttributeName("step")] public string? Step { get; set; }
    [HtmlAttributeName("min")] public string? Min { get; set; }
    [HtmlAttributeName("max")] public string? Max { get; set; }
    /// <summary>
    /// Defines the size of the field. Available for all types, except checkboxes.
    /// </summary>
    [HtmlAttributeName("size")] public Size FieldSize { get; set; } = Size.Normal;
    [HtmlAttributeName("readonly")] public bool Readonly { get; set; }
    [HtmlAttributeName("disabled")] public bool Disabled { get; set; }

    /// <summary>
    /// Specifies the autocomplete attribute of the input text.
    /// If set to false, the autocomplete will be set to 'off'.
    /// </summary>
    [HtmlAttributeName("auto-complete")] public bool? AutoComplete { get; set; }

    private bool IsCheckBox => Items == null && (For?.ModelExplorer.ModelType == typeof(bool) || For?.ModelExplorer.ModelType == typeof(bool?));
    private bool IsDateTime => Items == null && (For?.ModelExplorer.ModelType == typeof(DateTime) || For?.ModelExplorer.ModelType == typeof(DateTime?));
    private bool IsDateOnly => Items == null && (For?.ModelExplorer.ModelType == typeof(DateOnly) || For?.ModelExplorer.ModelType == typeof(DateOnly?));
    private bool IsTimeOnly => Items == null && (For?.ModelExplorer.ModelType == typeof(TimeOnly) || For?.ModelExplorer.ModelType == typeof(TimeOnly?));
    private bool IsSelectList => !IsCheckBoxList && Items != null && For != null;
    private bool IsCheckBoxList => Items != null && For != null && Checkboxes;
    private bool HasIcon => !string.IsNullOrWhiteSpace(Icon);
    private bool HasError => For != null && ViewContext.ModelState.GetValidationState(For.Name) == ModelValidationState.Invalid;
    private bool HasWarning => !string.IsNullOrWhiteSpace(Warning);
    private string SizeClass => FieldSize switch
    {
        Size.Small => " is-small",
        Size.Medium => " is-medium",
        Size.Large => " is-large",
        _ => ""
    };
    private string StateClass => HasError ? " is-danger" : HasWarning || IsWarning ? " is-warning" : "";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childContent = (await output.GetChildContentAsync()).GetContent();

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        var builder = new HtmlContentBuilder();

        // ReSharper disable MustUseReturnValue
        AppendLabel(builder);

        builder.AppendHtml($"<div class=\"field{(!string.IsNullOrWhiteSpace(Append + Prepend) ? " has-addons" : "")}\" style=\"margin-bottom:0;\">");
        if (!string.IsNullOrWhiteSpace(Prepend)) builder.AppendHtml($"<div class=\"control\"><a class=\"button is-static{SizeClass}\">{Prepend}</a></div>");
        builder.AppendHtml($"<div class=\"control is-expanded{(HasIcon ? " has-icons-left" : "")}\">");
        if (!string.IsNullOrWhiteSpace(childContent)) builder.AppendHtml(childContent);
        AppendSelect(builder);
        AppendCheckbox(builder);
        AppendCheckBoxList(builder);
        AppendTextbox(builder);
        builder.AppendHtml("</div>");
        if (!string.IsNullOrWhiteSpace(Append)) builder.AppendHtml($"<div class=\"control\"><a class=\"button is-static{SizeClass}\">{Append}</a></div>");
        builder.AppendHtml("</div>");
        AppendValidationSection(builder);
        AppendHelpSection(builder);
        AppendWarningSection(builder);
        // ReSharper restore MustUseReturnValue

        output.Content.SetHtmlContent(builder);
    }

    private void AppendLabel(IHtmlContentBuilder builder)
    {
        var labelText = Label;
        if (IsCheckBox) labelText = Placeholder;
        if (string.IsNullOrWhiteSpace(labelText)) return;

        if (For?.ModelExplorer == null)
        {
            builder.AppendHtml($"<label class=\"label{SizeClass}\">{labelText}</label>");
            return;
        }

        var label = _htmlGenerator.GenerateLabel(ViewContext, For?.ModelExplorer, For?.Name, labelText, new { @class = $"label{SizeClass}" });
        builder.AppendHtml(label);
    }

    private void AppendSelect(IHtmlContentBuilder builder)
    {
        if (!IsSelectList || For?.ModelExplorer == null) return;
        var select = _htmlGenerator.GenerateSelect(ViewContext, For?.ModelExplorer, null, For?.Name, Items, false, new { });
        if (Disabled) select.Attributes.Add("disabled", "disabled");
        if (Required) select.Attributes.Add("required", "required");
        builder.AppendHtml($"<div class=\"select is-fullwidth{SizeClass}{StateClass}\">");
        builder.AppendHtml(select);
        builder.AppendHtml("</div>");
        AppendIcon(builder);
    }

    private void AppendCheckbox(IHtmlContentBuilder builder)
    {
        if (!IsCheckBox || For?.ModelExplorer == null) return;
        var checkbox = _htmlGenerator.GenerateCheckBox(ViewContext, For?.ModelExplorer, For?.Name, For?.Model is true, new { });
        if (Readonly)
        {
            checkbox.Attributes.Add("readonly", "readonly");
            checkbox.Attributes.Add("onchange", "this.checked = this.readOnly ? !this.checked : this.checked");
        }
        if (Disabled) checkbox.Attributes.Add("disabled", "disabled");
        if (Required) checkbox.Attributes.Add("required", "required");
        builder.AppendHtml("<label class=\"checkbox\">");
        builder.AppendHtml(checkbox);
        builder.AppendHtml("&nbsp;&nbsp;");

        if (HasIcon)
        {
            var icon = Icon ?? "";
            if (!icon.StartsWith("fa", StringComparison.OrdinalIgnoreCase)) icon = $"fad fa-{icon}";
            builder.AppendHtml($"<i class=\"{icon}\"></i>");
            builder.AppendHtml("&nbsp;&nbsp;");
        }

        builder.AppendHtml(Label ?? "");
        builder.AppendHtml("</label>");
    }

    private void AppendIcon(IHtmlContentBuilder builder)
    {
        if (HasIcon)
        {
            var icon = Icon ?? "";
            if (!icon.StartsWith("fa", StringComparison.OrdinalIgnoreCase)) icon = $"fad fa-{icon}";
            builder.AppendHtml($"<span class=\"icon is-left{SizeClass}{StateClass}\"><i class=\"{icon}\"></i></span>");
        }
    }

    private void AppendCheckBoxList(IHtmlContentBuilder builder)
    {
        if (!IsCheckBoxList || For?.ModelExplorer == null || Items == null) return;

        IList<string> selectedItems = new List<string>();
        if (For.Model is IEnumerable e)
        {
            foreach (var l in e) selectedItems.Add(l.ToString() ?? "");
        }

        foreach (var item in Items)
        {
            builder.AppendHtml("<label class=\"checkbox\">");
            builder.AppendHtml($"<input type=\"checkbox\" value=\"{item.Value}\" name=\"{For.ModelExplorer.Metadata.Name}\" {(selectedItems.Any(x => x.Equals(item.Value, StringComparison.OrdinalIgnoreCase)) ? "checked" : "")} >");
            builder.AppendHtml("&nbsp;&nbsp;");

            if (HasIcon)
            {
                var icon = Icon ?? "";
                if (!icon.StartsWith("fa", StringComparison.OrdinalIgnoreCase)) icon = $"fad fa-{icon}";
                builder.AppendHtml($"<i class=\"{icon}\"></i>");
                builder.AppendHtml("&nbsp;&nbsp;");
            }

            builder.AppendHtml(item.Text ?? "");
            builder.AppendHtml("</label>");
        }
    }

    private void AppendTextbox(IHtmlContentBuilder builder)
    {
        if (IsSelectList || IsCheckBox || IsCheckBoxList || For?.ModelExplorer == null) return;

        TagBuilder? control = null;
        TagBuilder? invariantControl = null;

        var attributes = new Dictionary<string, object?>
        {
            { "class", $"input{SizeClass}{StateClass}" },
        };
        if (Step != null) attributes["step"] = Step;
        if (Placeholder != null) attributes["placeholder"] = Placeholder;
        if (Min != null) attributes["min"] = Min;
        if (Max != null) attributes["max"] = Max;
        if (Required) attributes["required"] = "required";
        if (Readonly) attributes["readonly"] = "readonly";
        if (Disabled) attributes["disabled"] = "disabled";
        if (AutoComplete == false) attributes["autocomplete"] = "off";

        if (Password)
        {
            control = _htmlGenerator.GeneratePassword(ViewContext, For?.ModelExplorer, For?.Name, null, attributes);
        }
        else if (MultiLine)
        {
            attributes["class"] = $"textarea{SizeClass}";
            control = _htmlGenerator.GenerateTextArea(ViewContext, For?.ModelExplorer, For?.Name, Rows, 0, attributes);
        }
        else if (FileUpload || MultipleFileUpload)
        {
            builder.AppendHtml($"<div class=\"file has-name is-fullwidth{SizeClass}\"><label class=\"file-label\">");

            var fileUploadAttributes = new Dictionary<string, object?>
            {
                { "type", "file" },
                { "class", "file-input" },
                { "accept", FileUploadAcceptFilter },
                { "onchange", "if (this.files.length > 0) { this.closest(\"div.file\").querySelector(\"span.file-name\").innerText = this.files[0].name; }"},
            };
            if (MultipleFileUpload)
            {
                fileUploadAttributes["multiple"] = "multiple";
                fileUploadAttributes["onchange"] = $"if (this.files.length > 0) {{ this.closest(\"div.file\").querySelector(\"span.file-name\").innerText = this.files.length > 1 ? `${{this.files.length}} {MultipleFileUploadFilesLabel}` : this.files[0].name; }}";
            }

            builder.AppendHtml(_htmlGenerator.GenerateTextBox(ViewContext, For?.ModelExplorer, For?.Name,
                For?.ModelExplorer.Model, null, fileUploadAttributes));

            builder.AppendHtml($"<span class=\"file-cta\"><span class=\"file-label\">{Placeholder}</span></span>");
            builder.AppendHtml("<span class=\"file-name\"></span></label></div>");
        }
        else if (IsDateTime)
        {
            attributes["type"] = "date";
            control = _htmlGenerator.GenerateTextBox(ViewContext, For?.ModelExplorer, For?.Name, (For?.ModelExplorer.Model as DateTime?)?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), null, attributes);
            // Generate an extra hidden field such that this field with be parsed with the invariant culture.
            // Cf. https://github.com/dotnet/aspnetcore/pull/43182
            invariantControl = GenerateInvariantCultureMetadata(ViewContext, For?.Name);
        }
        else if (IsDateOnly)
        {
            attributes["type"] = "date";
            control = _htmlGenerator.GenerateTextBox(ViewContext, For?.ModelExplorer, For?.Name, (For?.ModelExplorer.Model as DateOnly?)?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), null, attributes);
            // Generate an extra hidden field such that this field with be parsed with the invariant culture.
            // Cf. https://github.com/dotnet/aspnetcore/pull/43182
            invariantControl = GenerateInvariantCultureMetadata(ViewContext, For?.Name);
        }
        else if (IsTimeOnly)
        {
            attributes["type"] = "time";
            control = _htmlGenerator.GenerateTextBox(ViewContext, For?.ModelExplorer, For?.Name, (For?.ModelExplorer.Model as TimeOnly?)?.ToString("HH:mm", CultureInfo.InvariantCulture), null, attributes);
            // Generate an extra hidden field such that this field with be parsed with the invariant culture.
            // Cf. https://github.com/dotnet/aspnetcore/pull/43182
            invariantControl = GenerateInvariantCultureMetadata(ViewContext, For?.Name);
        }
        else if (Number)
        {
            attributes["type"] = "number";
            control = _htmlGenerator.GenerateTextBox(ViewContext, For?.ModelExplorer, For?.Name, For?.ModelExplorer.Model, Format, attributes);
            // Generate an extra hidden field such that this field with be parsed with the invariant culture.
            // Cf. https://github.com/dotnet/aspnetcore/pull/43182
            invariantControl = GenerateInvariantCultureMetadata(ViewContext, For?.Name);
        }
        else
        {
            control = _htmlGenerator.GenerateTextBox(ViewContext, For?.ModelExplorer, For?.Name, For?.ModelExplorer.Model, Format, attributes);
        }

        if (control != null)
        {
            builder.AppendHtml(control);
            if (invariantControl != null) builder.AppendHtml(invariantControl);
        }

        AppendIcon(builder);
    }

    private static TagBuilder? GenerateInvariantCultureMetadata(ViewContext context, string? propertyName)
    {
        if (propertyName == null) return null;
        var builder = new TagBuilder("input");
        builder.Attributes.Add("type", "hidden");
        builder.Attributes.Add("name", CultureInvariantFieldName);
        var prefix = context.ViewData.TemplateInfo.HtmlFieldPrefix;
        var value = !string.IsNullOrWhiteSpace(prefix) ? $"{prefix}.{propertyName}" : propertyName;
        builder.Attributes.Add("value", value);
        builder.TagRenderMode = TagRenderMode.SelfClosing;
        return builder;
    }

    private void AppendValidationSection(IHtmlContentBuilder builder)
    {
        if (For == null) return;
        var validation = _htmlGenerator.GenerateValidationMessage(ViewContext, For.ModelExplorer, For.Name, null, "div", new { @class = $"help is-danger{SizeClass}" });
        builder.AppendHtml(validation);
    }

    private void AppendHelpSection(IHtmlContentBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Help)) return;
        builder.AppendHtml("<div class=\"help\">");
        builder.AppendHtml(Help);
        builder.AppendHtml("</div>");
    }

    private void AppendWarningSection(IHtmlContentBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Warning)) return;
        builder.AppendHtml("<div class=\"help is-warning\">");
        builder.AppendHtml(Warning);
        builder.AppendHtml("</div>");
    }

    public enum Size
    {
        Small,
        Normal,
        Medium,
        Large
    }
}
