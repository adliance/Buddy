# Adliance.AspNetCore.Buddy.Template.Razor

This razor templating library offers several rendering options of templates.

- Render a template to HTML (what Razor usually does)
- Render a template and send it as email (HTML and text version).
- Generate a PDF with header and footer.

## Setup library in an ASP.NET project

When adding the Razor templating mechanism, it's necessary to add the MVC services for View support to the ASP.NET project with the `AddControllersWithViews` extension method.

The `IBuddyServiceCollection` of the `Adliance.AspNetCore.Buddy.Abstractions` package offers the `AddBuddy` method, which several extensions to add the templating and PDF services.

- `AddRazorTemplater()`
- `AddPdf(configuration)`
- `AddRazorPdfV1Renderer()`
- `AddRazorPdfV2Renderer()`

```c#
public void ConfigureServices(IServiceCollection services)
{
  //...
  services.AddBuddy()
    .AddRazorTemplater()
    .AddPdf(Configuration.GetSection("Pdf"))    
    .AddRazorPdfV2Renderer();
  //Don't forget to add MVC services to your project.
  services.AddControllersWithViews();
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

There are following three possibilities to use the library. 

### Render HTML

In the `Views` directory in your project, you have to add a new directory containing the template you want to render.

The structure would look like:

- Views
  - DirectoryName
    - {TemplateName}.cshtml

The usage looks like:

```c#
ITemplate _templater = 
await _templater.Render("DirectoryName", "TemplateName", viewModel);
```

Provide the name of the directory and the template. Usually a template has a view model, which is the third parameter.

### Render and send an email

To use the full functionality of this option consider using another buddy package `Adliance.AspNetCore.Buddy.Email.Mailjet` to send the rendered templates or implement the email logic by yourself.

In the `Views` directory in your project, you have to add a new `EmailTemplates` directory. Each "email" consists of three templates, one for the subject, two for the content (HTML and text).
The structure would look like:

- Views
  - EmailTemplates
    - {TemplateBaseName}.Subject.cshtml
    - {TemplateBaseName}.Html.cshtml
    - {TemplateBaseName}.Text.cshtml
  
Use the email renderer by providing the recipient as first parameter, the name of the base template as second and the viewModel as last.

```c#
await _emailRenderer.RenderAndSend("recipient@adliance.net", "TemplateBaseName", viewModel);
```

### Render a PDF

This option uses the `Adliance.AspNetCore.Buddy.Pdf` package to generate PDFs.

To render a PDF template, you need to provide four `cshtml` files in a `PdfTemplates` directory in the `Views` directory of your project.

The default template structure is:

- Views
  - PdfTemplates
    - {TemplateBaseName}.cshtml
    - {TemplateBaseName}.Filename.cshtml
    - {TemplateBaseName}.Header.cshtml
    - {TemplateBaseName}.Footer.cshtml
  
This code sample shows the usage of the PDF renderer in a controller action, which is directly returning the resulting PDF as file content.
```c#
PdfRendererResult result = await _pdfRenderer.Render("TemplateBaseName", viewModel);
return new FileContentResult(result.Bytes, MediaTypeNames.Application.Pdf)
{
	FileDownloadName = result.Filename
};
```

### PDF library documentation

See more detailed information in the [PDF library](../Adliance.AspNetCore.Buddy.Pdf/readme.md)


## Razor Tag Helpers

Add the Buddy Tag Helper to your `_ViewImports.cshtml` file.
```c#
@using System
@using System.Collections.Generic

@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, Adliance.AspNetCore.Buddy.Template.Razor
```
