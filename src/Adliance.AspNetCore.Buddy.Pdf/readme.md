# Adliance.AspNetCore.Buddy.Pdf

This PDF library creates a PDF from HTML with header and footer if provided.
The Pdf library provides two versions of PDF generation.

- Version 1 (`Adliance.AspNetCore.Buddy.Pdf.V1`) uses [wkhtmltopdf](https://wkhtmltopdf.org/)
- Version 2 (`Adliance.AspNetCore.Buddy.Pdf.V2`) uses [pdf-lib](https://pdf-lib.js.org/)

## Setup library in an ASP.NET project

The `IBuddyServiceCollection` of the `Adliance.AspNetCore.Buddy.Abstractions` package offers the `AddBuddy` method, which provides several extensions to add the PDF services.
The library has two versions of the PDF service in following namespaces:

- `using Adliance.AspNetCore.Buddy.Pdf.V1;`
- `using Adliance.AspNetCore.Buddy.Pdf.V2;`
  
The methods are the same in every namespace, so be aware of using the correct `using` statements when adding the PDF service with the `AddPdf(configuration)` method.

```c#
public void ConfigureServices(IServiceCollection services)
{
  //...
  services.AddBuddy()
    .AddPdf(Configuration.GetSection("Pdf"))    
  //...
 }
```

### Configuration (appsettings.json)

Add a section in the configuration of your project and point to the used PDF service endpoint.

```json
{
  "Pdf": {
    "ServerUrl": "https://url-to-pdf.service"
  }
}
```

### Health check

The library also offers an extension to the `IHealthChecksBuilder` to add a health check to the used PDF service.

```c#
services.AddHealthChecks()
  .AddPdfCheck();
```

## Usage of library

### Generate a PDF

This code sample shows the usage of the PDF generator. Just call the `HtmlToPdf` method with the HTML string and the PDF options.
```c#
IPdfer _pdfer = new AdliancePdfer(new DefaultPdferConfiguration());
byte[] bytes = await _pdfer.HtmlToPdf("This is <b>my</b> <u>HTML</u> code.", new PdfOptions());
```

### Generate a PDF from a template
Generating a PDF from a Handlebars template works similar. Just call the `TemplateToPdf` method and provide the template, the model and `TemplateOptions` for advanced functionality.
```c#
IPdfer _pdfer = new AdliancePdfer(new DefaultPdferConfiguration());
byte[] bytes = await _pdfer.TemplateToPdf("<b>Hello</b> from {{Name}}", new { Name = "Model" }, new TemplateOptions());
```

### Add one or more watermarks to an existing PDF
Stamps one or more watermarks onto every page of an existing PDF (e.g. one previously produced by `HtmlToPdf`), each rendered independently and applied in list order. Like header/footer, each watermark's HTML is rendered once per page, so it supports the same `.current-page`/`.total-pages` substitution and page-conditional classes (`.only-on-first-page`, `.not-on-first-page`).

```c#
IPdfer _pdfer = new AdliancePdfer(new DefaultPdferConfiguration());
byte[] pdf = await _pdfer.HtmlToPdf("This is <b>my</b> HTML.", new PdfOptions());
byte[] watermarked = await _pdfer.AddWatermark(pdf,
[
    new WatermarkOptions { Html = "<div style='width:100%;height:100%;background-color:rgba(255,0,0,0.3);'>CONFIDENTIAL</div>" }
]);
```

Watermarks can also be added directly while generating a PDF, by setting `PdfOptions.Watermarks` (or `TemplateOptions.Watermarks`, since it inherits from `PdfOptions`):

```c#
byte[] bytes = await _pdfer.HtmlToPdf("This is <b>my</b> HTML.", new PdfOptions
{
    Watermarks = [new WatermarkOptions { Html = "<div style='width:100%;height:100%;background-color:rgba(255,0,0,0.3);'>DRAFT</div>" }]
});
```

There is no separate opacity option — transparency is controlled entirely by the watermark's own HTML/CSS, exactly like header/footer/body HTML already work. Two common ways to do it, with different visual effects:

```html
<!-- alpha channel on the background color: only the fill is translucent, any text/foreground
     content drawn on top (if given a solid opaque color) stays fully opaque -->
<div style='width:100%;height:100%;background-color:rgba(255,0,0,0.3);'>CONFIDENTIAL</div>

<!-- the CSS opacity property: applies to the whole element as one composited group, so
     background AND text both become translucent together -->
<div style='width:100%;height:100%;opacity:0.3;background-color:red;'>CONFIDENTIAL</div>
```

Pick whichever matches the effect you want. This was a deliberate choice: applying opacity server-side (by injecting it into the caller's HTML before rendering) proved unreliable in practice — it broke for full `<!DOCTYPE html>` documents, and separately for a background set directly on `<body>` (a very common pattern), since CSS propagates that to the page canvas in a way that bypasses element-level opacity. Rather than depend on watermark HTML never using these realistic patterns, the caller is left in full control instead. That `<body>` caveat applies regardless of which of the two forms above you use, if you put the background there instead of on a nested element.

**Note on tamper-resistance:** a watermark added this way is stamped directly into each page's content stream (the same mechanism used for header/footer), so it is visually persistent and not a toggleable annotation layer — but it is not cryptographic protection. Anyone with PDF-editing tools can still remove or obscure it. Use this for visible marking/branding purposes (e.g. "DRAFT", "CONFIDENTIAL"), not as an access-control or anti-tamper mechanism.

### PDF Options

#### Version 2

| Name         | Type     | Description                            |
|--------------|----------|----------------------------------------|
| HeaderHtml   | `string` | The HTML for the PDF header as string. |
| HeaderHeight | `int`    | The height of the header in pixel (px). If a HeaderHtml is provided, the height must be set. |
| FooterHtml   | `string` | The HTML for the PDF footer as string. |
| FooterHeight | `int`    | The height of the footer in pixel (px). If a FooterHtml is provided, the height must be set. |
| Watermarks   | `IList<WatermarkOptions>` | The watermarks to stamp onto the PDF, each once per page, applied in list order. Optional — omit or leave empty to skip watermarking. |

### Watermark Options

| Name    | Type     | Description                            |
|---------|----------|----------------------------------------|
| Html    | `string` | The HTML for the watermark as string. Required. Transparency is controlled directly in this HTML/CSS (e.g. `opacity: 0.3`, `rgba()`/`hsla()` colors) — there is no separate opacity option. |
| X       | `int`    | The horizontal position (px) of the watermark, from the page's top-left corner. Defaults to 0. |
| Y       | `int`    | The vertical position (px) of the watermark, from the page's top-left corner. Defaults to 0. |
| Width   | `int`    | The width (px) of the box the watermark HTML is rendered into. Defaults to the full page width. |
| Height  | `int`    | The height (px) of the box the watermark HTML is rendered into. Defaults to the full page height. |

### Template Options

The `TemplateOptions` class extends `PdfOptions` and provides some configuration for the resulting PDF.

| Name         | Type     | Description                            |
|--------------|----------|----------------------------------------|
| JavaScript   | `string` | The optional JavaScript code to modify the provided model. |
| HeaderHtml   | `string` | The Handlebars template for the PDF header as string. |
| HeaderModel  | `string` | The model for the `HeaderHtml`. |
| HeaderJavaScript | `string` | The optional Javascript used to modify the `HeaderModel`. |
| HeaderHeight | `int`    | The height of the header in pixel (px). If a `HeaderHtml` is provided, the height must be set. |
| FooterHtml   | `string` | The Handlebars template for the PDF footer as string. |
| FooterModel  | `string` | The model for the `FooterHtml`. |
| FooterJavaScript | `string` | The optional Javascript used to modify the `FooterModel`. |
| FooterHeight | `int`    | The height of the footer in pixel (px). If a `FooterHtml` is provided, the height must be set. |

## Useful information
### Page numbers
To have page numbers, e.g in the footer of the PDF, there are two CSS classes. If placed on a span element, the content of the element is substituted with the appropriate value.

- `current-page`: Specifies the current page number. 
- `total-pages`: The total number of pages, which will be generated.

A HTML snippet could look like this:
```html
<span class="current-page"></span> / <span class="total-pages"></span>
```
