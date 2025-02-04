using Adliance.AspNetCore.Buddy.Testing.Extensions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithoutDatabase;

public class InProcessTest (WithoutDatabaseFixture<InProcessOptions> fixture) : BaseTest<InProcessOptions>(fixture)
{
}

public class InContainerTest(WithoutDatabaseFixture<InContainerOptions> fixture) : BaseTest<InContainerOptions>(fixture)
{
}

public abstract class BaseTest<TOptions> : IClassFixture<WithoutDatabaseFixture<TOptions>> where TOptions : IFixtureOptions, new()
{
    protected readonly WithoutDatabaseFixture<TOptions> Fixture;

    protected BaseTest(WithoutDatabaseFixture<TOptions> fixture)
    {
        Fixture = fixture;
    }
    
    [Fact]
    public async Task Can_Get_Home()
    {
        var response = await Fixture.Client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("This is our view.", responseString);
    }

    [Fact]
    public async Task Can_Post_Home()
    {
        var response = await Fixture.Client.PostAsync("/", new FormUrlEncodedContent(new Dictionary<string, string>
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
        if (Fixture.Options.Playwright == PlaywrightOptions.None) return; // Assert.Skip is only available in XUnit 3

        await Fixture.Page.Navigate(Fixture.Client);
        var pageContent = await Fixture.Page.ContentAsync();
        await Fixture.Page.Screenshot("home");
        Assert.Contains("This is our view.", pageContent);
    }
    
    [Fact]
    public async Task Can_Make_Second_Screenshot_of_Home()
    {
        if (Fixture.Options.Playwright == PlaywrightOptions.None) return; // Assert.Skip is only available in XUnit 3

        await Fixture.Page.Navigate(Fixture.Client);
        var pageContent = await Fixture.Page.ContentAsync();
        await Fixture.Page.Screenshot("home");
        Assert.Contains("This is our view.", pageContent);
    }
    
    [Fact]
    public async Task Can_Make_Third_Screenshot_of_Home()
    {
        if (Fixture.Options.Playwright == PlaywrightOptions.None) return; // Assert.Skip is only available in XUnit 3

        await Fixture.Page.Navigate(Fixture.Client);
        var pageContent = await Fixture.Page.ContentAsync();
        await Fixture.Page.Screenshot("home");
        Assert.Contains("This is our view.", pageContent);
    }
}