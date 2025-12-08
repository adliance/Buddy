using Adliance.AspNetCore.Buddy.Testing.Shared;
using Adliance.AspNetCore.Buddy.Testing.Shared.Extensions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithoutDatabase;

public class InProcessTest(WithoutDatabaseFixture<InProcessOptions> fixture) : BaseTest<InProcessOptions>(fixture);

public class InContainerTest(WithoutDatabaseFixture<InContainerOptions> fixture) : BaseTest<InContainerOptions>(fixture);

public abstract class BaseTest<TOptions>(WithoutDatabaseFixture<TOptions> fixture) : IClassFixture<WithoutDatabaseFixture<TOptions>>
    where TOptions : BuddyFixtureOptions<Program>, new()
{
    [Fact]
    public async Task Can_Get_Home()
    {
        var response = await fixture.Client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("This is our view.", responseString);
    }

    [Fact]
    public async Task Can_Post_Home()
    {
        var response = await fixture.Client.PostAsync("/", new FormUrlEncodedContent(new Dictionary<string, string>
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
        if (fixture.Options.Playwright == null) return; // Assert.Skip is only available in XUnit 3

        await fixture.Page.Navigate(fixture.Client);
        var pageContent = await fixture.Page.ContentAsync();
        await fixture.Page.Screenshot("home_1");
        Assert.Contains("This is our view.", pageContent);
    }

    [Fact]
    public async Task Can_Make_Second_Screenshot_of_Home()
    {
        if (fixture.Options.Playwright == null) return; // Assert.Skip is only available in XUnit 3

        await fixture.Page.Navigate(fixture.Client);
        var pageContent = await fixture.Page.ContentAsync();
        await fixture.Page.Screenshot("home_2");
        Assert.Contains("This is our view.", pageContent);
    }

    [Fact]
    public async Task Can_Make_Third_Screenshot_of_Home()
    {
        if (fixture.Options.Playwright == null) return; // Assert.Skip is only available in XUnit 3

        await fixture.Page.Navigate(fixture.Client);
        var pageContent = await fixture.Page.ContentAsync();
        await fixture.Page.Screenshot("home_3");
        Assert.Contains("This is our view.", pageContent);
    }
}
