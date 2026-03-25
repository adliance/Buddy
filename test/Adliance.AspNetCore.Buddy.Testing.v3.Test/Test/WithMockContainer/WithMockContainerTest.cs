using Adliance.AspNetCore.Buddy.Testing.Shared;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.v3.Test.Test.WithMockContainer;

public class WithMockContainerTest(WithMockContainerFixture<InContainerOptions> fixture) : BaseTest<InContainerOptions>(fixture);

public abstract class BaseTest<TOptions>(WithMockContainerFixture<TOptions> fixture)
    : IClassFixture<WithMockContainerFixture<TOptions>>
    where TOptions : BuddyFixtureOptions<Program>, new()
{
    [Fact]
    public async Task Can_Access_Mock_Api()
    {
        var apiClient = fixture.InContainers.First().Client;

        var response = await apiClient.GetAsync("/api/hello", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        Assert.Equal("Hello from the mock API!", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

}