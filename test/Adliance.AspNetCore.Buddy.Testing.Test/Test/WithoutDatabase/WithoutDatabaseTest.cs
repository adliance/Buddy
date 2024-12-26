using Adliance.AspNetCore.Buddy.Testing.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithoutDatabase;

public class InProcessTest(WebApplicationFactory<Program> factory) : BaseTest<InProcessOptions>(factory)
{
}

public class InContainerTest() : BaseTest<InContainerOptions>(null!)
{
}

public abstract class BaseTest<TOptions>(WebApplicationFactory<Program>? factory)
    : BuddyFixture<TOptions, Program>(factory) where TOptions : IFixtureOptions, new()
{
    [Fact]
    public async Task Can_Get_Home()
    {
        var response = await Client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("This is our view.", responseString);
    }

    [Fact]
    public async Task Can_Post_Home()
    {
        var response = await Client.PostAsync("/", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["postContent"] = "My post content."
        }));
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("My post content.", responseString);
    }

    [Fact]
    public async Task Can_Make_Screenshot_of_Home()
    {
        if (Options.Playwright == PlaywrightOptions.None) return; // Assert.Skip is only available in XUnit 3

        await Page.Navigate(Client);
        var pageContent = await Page.ContentAsync();
        await Page.Screenshot("home");
        Assert.Contains("This is our view.", pageContent);
    }
}