namespace Adliance.AspNetCore.Buddy.Pdf.V2;

public class TemplateOptions
{
    public required string Template { get; set; }

    public object? Model { get; set; }

    public string? Javascript { get; set; }
}

public class HeaderTemplateOptions : TemplateOptions
{
    public int Height { get; set; }
}

public class FooterTemplateOptions : TemplateOptions
{
    public int Height { get; set; }
}
