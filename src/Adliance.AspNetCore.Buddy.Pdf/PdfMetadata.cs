using System.Text.Json.Serialization;

namespace Adliance.AspNetCore.Buddy.Pdf;

public class PdfMetadata
{
    [JsonPropertyName("total_pages")] public int TotalPages { get; set; }
    [JsonPropertyName("outline")] public OutlineData[] Outline { get; set; } = [];

    public class OutlineData
    {
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("page")] public int Page { get; set; }
        [JsonPropertyName("children")] public OutlineData[] Children { get; set; } = [];
    }
}
