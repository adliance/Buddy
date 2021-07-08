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

