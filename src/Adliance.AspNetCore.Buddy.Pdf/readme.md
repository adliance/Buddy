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

### PDF Options

#### Version 2

| Name         | Type     | Description                            |
|--------------|----------|----------------------------------------|
| HeaderHtml   | `string` | The HTML for the PDF header as string. |
| HeaderHeight | `int`    | The height of the header in pixel (px). If a HeaderHtml is provided, the height must be set. |
| FooterHtml   | `string` | The HTML for the PDF footer as string. |
| FooterHeight | `int`    | The height of the footer in pixel (px). If a FooterHtml is provided, the height must be set. |

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
