using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.InContainerNoSqlServer;

public class InProcessNoSqlServerTest : BuddyFixture<FixtureOptions, Program>
{
    [Fact]
    public async Task Get_Get_Home()
    {
        var response = await Client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("This is our view.", responseString);
    }
    
    [Fact]
    public async Task Get_Post_Home()
    {
        var response = await Client.PostAsync("/", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["postContent"] = "My post content."
        }));
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("My post content.", responseString);
    }
}


