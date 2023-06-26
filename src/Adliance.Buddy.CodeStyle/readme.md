# Adliance.Buddy.CodeStyle

This package is used to distribute a common *.editorconfig* file. During the build an *.editorconfig* file is copied from the nuget package to the project folder.

## Usage 

Add a *Directory.Build.props* file to your solution root with the following content. Replace the version by the desired nuget package version.
Now the Nuget package is installed to all projects. 

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

It is recommended to ignore the copied *.editorconfig* files via your *.gitignore* file.
