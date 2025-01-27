using Adliance.AspNetCore.Buddy.Testing.Extensions;
using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InProcessTest(WithDatabaseFixture<InProcessOptions> fixture) : BaseTest<InProcessOptions>(fixture)
{
}

public class InContainerTest(WithDatabaseFixture<InContainerOptions> fixture) : BaseTest<InContainerOptions>(fixture)
{
}

public abstract class BaseTest<TOptions> : IClassFixture<WithDatabaseFixture<TOptions>> where TOptions : IFixtureOptions, new()
{
    protected readonly WithDatabaseFixture<TOptions> Fixture;

    protected BaseTest(WithDatabaseFixture<TOptions> fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public async Task Can_Get_Database()
    {
        await Fixture.Db.Table.ExecuteDeleteAsync();
        for (var i = 1; i <= 100; i++)
            await Fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            });
        await Fixture.Db.SaveChangesAsync();

        var response = await Fixture.Client.GetAsync("/Home/Database");
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("There are 100 rows in the database.", responseString);
    }
    
    [Fact]
    public async Task Can_Get_Database_A_Second_time()
    {
        await Fixture.Db.Table.ExecuteDeleteAsync();
        for (var i = 1; i <= 500; i++)
            await Fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            });
        await Fixture.Db.SaveChangesAsync();

        var response = await Fixture.Client.GetAsync("/Home/Database");
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("There are 500 rows in the database.", responseString);
    }

    [Fact]
    public async Task Can_Make_Screenshot_of_Database()
    {
        if (Fixture.Options.Playwright == PlaywrightOptions.None) return; // Assert.Skip is only available in XUnit 3
        await Fixture.Db.Table.ExecuteDeleteAsync();
        for (var i = 1; i <= 50; i++)
            await Fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            });
        await Fixture.Db.SaveChangesAsync();

        await Fixture.Page.Navigate(Fixture.Client, "/Home/Database");
        var pageContent = await Fixture.Page.ContentAsync();
        await Fixture.Page.Screenshot("database");
        Assert.Contains("There are 50 rows in the database.", pageContent);
    }
}