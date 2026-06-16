# Adliance.AspNetCore.Buddy.GuiKit
The GuiKit Buddy provides a set of UI components, tag helpers, and static assets (CSS, JS, fonts) for building consistent ASP.NET Core web applications.

## Features
- Bundled and minified CSS and JavaScript assets
- Razor tag helpers for common UI components
- Application Insights integration helper

## Setup
Register the services in your ASP.NET Core application:

```csharp
using Adliance.AspNetCore.Buddy.GuiKit.Extensions;

// In Program.cs or Startup.ConfigureServices:
services.AddApplicationInsightsIfConfigured();
```

`AddApplicationInsightsIfConfigured` reads the `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable
and configures Application Insights telemetry automatically if it is present. It also registers `AppInsightsJavaScript`,
which can be injected into Razor views to render the Application Insights JavaScript snippet.

## Tag Helpers
Add the tag helpers to your `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, Adliance.AspNetCore.Buddy.GuiKit
```

| Tag Helper           | Description                            |
|----------------------|----------------------------------------|
| `<agk-loading>`      | Renders a loading indicator            |
| `<agk-notification>` | Renders a notification/alert component |
| `<agk-panel>`        | Renders a panel/card component         |
| `<agk-title>`        | Renders a styled page title            |

## Application Insights JavaScript
To inject the Application Insights JavaScript snippet into your layout, inject `AppInsightsJavaScript` and render the script tag:

```cshtml
@inject Adliance.AspNetCore.Buddy.GuiKit.AppInsightsJavaScript AppInsightsJavaScript

@Html.Raw(AppInsightsJavaScript.Script)
```
