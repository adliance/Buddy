# Adliance.AspNetCore.Buddy.GuiKit.BuildTasks
The GuiKit.BuildTasks package provides MSBuild tasks for bundling and minifying CSS and JavaScript files at build time.

## Features
- Bundle and minify multiple CSS files into a single output file
- Bundle and minify multiple JavaScript files into a single output file
- Integrates with `dotnet watch` to trigger rebuilds on source file changes

## Usage
Reference the package and declare `CssBundle` and/or `JsBundle` items in your `.csproj`:

```xml
<ItemGroup>
  <CssBundle OutputFile="wwwroot/css/bundle.css" Include="css/reset.css" />
  <CssBundle OutputFile="wwwroot/css/bundle.css" Include="css/main.css" />
</ItemGroup>

<ItemGroup>
  <JsBundle OutputFile="wwwroot/js/bundle.js" Include="js/vendor.js" />
  <JsBundle OutputFile="wwwroot/js/bundle.js" Include="js/app.js" />
</ItemGroup>
```

Files in each group are concatenated in declaration order and written to the `OutputFile` path. Multiple bundles with different `OutputFile` values can coexist in the same project.

The tasks run automatically after the `Build` target.
