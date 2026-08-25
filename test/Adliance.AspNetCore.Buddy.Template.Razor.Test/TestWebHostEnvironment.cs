using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

/// <summary>
/// The only faked piece for RazorTemplaterTest: an IWebHostEnvironment pointing at the RazorFixtures folder
/// shipped alongside this test assembly. Everything downstream of it - view location, compilation and
/// rendering - is the real ASP.NET Core Razor engine, so it exercises actual .cshtml files.
/// </summary>
public class TestWebHostEnvironment : IWebHostEnvironment
{
    public TestWebHostEnvironment()
    {
        ContentRootPath = Path.Combine(AppContext.BaseDirectory, "RazorFixtures");
        ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
    }

    public string EnvironmentName { get; set; } = "Development";
    public string ApplicationName { get; set; } = typeof(TestWebHostEnvironment).Assembly.GetName().Name!;
    public string WebRootPath { get; set; } = "";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string ContentRootPath { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
}
