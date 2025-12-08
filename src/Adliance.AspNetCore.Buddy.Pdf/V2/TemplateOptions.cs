using System.Text.Json.Serialization;

namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public class TemplateOptions : PdfOptions
{
    [JsonPropertyName("js")] public string? JavaScript { get; set; }
    [JsonPropertyName("header_js")] public string? HeaderJavaScript { get; set; }
    [JsonPropertyName("footer_js")] public string? FooterJavaScript { get; set; }
    [JsonPropertyName("header_model")] public object? HeaderModel { get; set; }
    [JsonPropertyName("footer_model")] public object? FooterModel { get; set; }
}
