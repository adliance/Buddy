using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

/// <summary>
/// Exercises RazorTemplater against the real ASP.NET Core Razor view engine, using runtime compilation of
/// the actual .cshtml files under RazorFixtures/Views. The only test double involved is the
/// IWebHostEnvironment pointing at that fixture folder; view lookup, compilation and rendering are all real.
/// </summary>
public class RazorTemplaterTest
{
    private static RazorTemplater CreateSut()
    {
        // MVC's Razor page activator needs a DiagnosticSource, which a real host normally registers for us.
        var diagnosticListener = new DiagnosticListener(nameof(RazorTemplaterTest));

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(diagnosticListener);
        services.AddSingleton<DiagnosticSource>(diagnosticListener);
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());
        services.AddControllersWithViews().AddRazorRuntimeCompilation();
        var provider = services.BuildServiceProvider();

        return new RazorTemplater(
            provider.GetRequiredService<IRazorViewEngine>(),
            provider.GetRequiredService<ITempDataProvider>(),
            provider);
    }

    [Fact]
    public async Task RendersAnActualRazorTemplate()
    {
        var sut = CreateSut();

        var result = await sut.Render("Greetings", "Simple", new RazorFixtureModel { Name = "World" });

        Assert.Equal("Hello World!", result.Trim());
    }

    [Fact]
    public async Task RendersRealRazorControlFlowAndHtmlEncoding()
    {
        var sut = CreateSut();

        var result = await sut.Render("Greetings", "List", new RazorFixtureModel { Name = "World", Items = ["<script>", "beer & pretzels"] });

        // the loop actually ran, and Razor's default HTML-encoding actually applied - not just string interpolation.
        Assert.Contains("<li>&lt;script&gt;</li>", result);
        Assert.Contains("<li>beer &amp; pretzels</li>", result);
    }

    [Fact]
    public async Task LooksUpTheViewInTheGivenDirectory()
    {
        var sut = CreateSut();

        var greeting = await sut.Render("Greetings", "Simple", new RazorFixtureModel { Name = "World" });
        var farewell = await sut.Render("Farewells", "Simple", new RazorFixtureModel { Name = "World" });

        // same template name, different directories -> the directory actually drives view resolution.
        Assert.Equal("Hello World!", greeting.Trim());
        Assert.Equal("Goodbye World!", farewell.Trim());
    }

    [Fact]
    public async Task ThrowsIfNoViewMatchesTheTemplateName()
    {
        var sut = CreateSut();

        var ex = await Assert.ThrowsAsync<Exception>(() => sut.Render("Greetings", "DoesNotExist", new RazorFixtureModel { Name = "World" }));
        Assert.Contains("DoesNotExist", ex.Message);
    }
}
