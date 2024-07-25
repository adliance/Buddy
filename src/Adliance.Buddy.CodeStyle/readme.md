# Adliance.Buddy.CodeStyle

This package is used to distribute a common *.editorconfig* file. During the build an *.editorconfig* file is copied from the nuget package to the project folder.

## Usage 

Add a *Directory.Build.props* file to your solution root with the following content. Replace the version by the desired nuget package version.
Now the Nuget package will be installed to all projects. Alternatively the nuget package can also be directly installed in each project if desired. 

```xml
<Project> 
  <PropertyGroup>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild> <!--This is optional. If set to true the code style is also enforced during build.-->
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Adliance.Buddy.CodeStyle" Version="1.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles;analyzers</IncludeAssets>
    </PackageReference>
 </ItemGroup>
</Project>
```

The copied *.editorconfig* files from the nuget package should be ignored via your *.gitignore* file.

## dotnet format
The `dotnet format` command also picks up the .editorconfig files.

It can be used to format and apply fixes automatically wherever possible.

`dotnet format --verify-no-changes` can be used to verify if there are potential issues or not.

https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format


## EnforceCodeStyleInBuild

When the CodeStyle is enforced during build the warnings/errors are only displayed on clean builds.

Possibility for clean builds:
- use `dotnet clean` before `dotnet build`
- delete `bin` and `obj` directories
- `dotnet build --no-incremental`

Incremental builds do not show any warnings/errors as the build is potentially cached locally.

## CI

The .editorconfig files must be in place in all project directories before formatting/building in a CI environment to ensure it is picked up correctly.

This can be done for each project with these commands:

1. restore all packages with `dotnet restore`
2. execute the target from the nuget package manually to copy the .editorconfig file `dotnet msbuild /t:CopyEditorConfig`


This can be executed like this in an Azure DevOps Pipeline:

```
steps:
  - task: DotNetCoreCLI@2
    displayName: 'Restore'
    inputs:
      command: restore
      projects: '**/*.csproj'
  - task: DotNetCoreCLI@2
    displayName: 'CopyEditorConfig'
    inputs:
      command: custom
      custom: 'msbuild'
      arguments: '/t:CopyEditorConfig'
      projects: '**/*.csproj'`
  - task: DotNetCoreCLI@2
    displayName: 'Lint'
    inputs:
      command: custom
      projects: '**/*.csproj'
      custom: format
      arguments: '-v d --verify-no-changes'
```
