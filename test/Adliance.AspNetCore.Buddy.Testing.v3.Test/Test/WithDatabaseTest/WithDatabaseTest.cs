using Adliance.AspNetCore.Buddy.Testing.Shared;
using Adliance.AspNetCore.Buddy.Testing.Shared.Extensions;
using Adliance.AspNetCore.Buddy.Testing.v3.Test.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.v3.Test.Test.WithDatabaseTest;

public class InProcessTest(WithDatabaseFixture<InProcessOptions> fixture) : BaseTest<InProcessOptions>(fixture);

public class InContainerTest(WithDatabaseFixture<InContainerOptions> fixture) : BaseTest<InContainerOptions>(fixture);

public abstract class BaseTest<TOptions>(WithDatabaseFixture<TOptions> fixture) : IClassFixture<WithDatabaseFixture<TOptions>>
    where TOptions : BuddyFixtureOptions<Program>, new()
{
    [Fact]
    public async Task Can_Get_Database()
    {
        await fixture.Db.Table.ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);
        for (var i = 1; i <= 100; i++)
            await fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            }, TestContext.Current.CancellationToken);
        await fixture.Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await fixture.Client.GetAsync("/Home/Database", TestContext.Current.CancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        Assert.Contains("There are 100 rows in the database.", responseString);
    }

    [Fact]
    public async Task Can_Get_Database_A_Second_time()
    {
        await fixture.Db.Table.ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);
        for (var i = 1; i <= 500; i++)
            await fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            }, TestContext.Current.CancellationToken);
        await fixture.Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await fixture.Client.GetAsync("/Home/Database", TestContext.Current.CancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        Assert.Contains("There are 500 rows in the database.", responseString);
    }

    [Fact]
    public async Task Can_Make_Screenshot_of_Database()
    {
        Assert.SkipWhen(fixture.Options.Playwright == null, "Playwright disabled.");
        await fixture.Db.Table.ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);
        for (var i = 1; i <= 1; i++)
            await fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            }, TestContext.Current.CancellationToken);
        await fixture.Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await fixture.Page.Navigate(fixture.Client, "/Home/Database");
        var pageContent = await fixture.Page.ContentAsync();
        await fixture.Page.Screenshot("database");
        Assert.Contains("There are 1 rows in the database.", pageContent);
    }

    [Fact]
    public async Task Can_Make_Second_Screenshot_of_Database()
    {
        Assert.SkipWhen(fixture.Options.Playwright == null, "Playwright disabled.");

        await fixture.Db.Table.ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);
        for (var i = 1; i <= 2; i++)
            await fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            }, TestContext.Current.CancellationToken);
        await fixture.Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await fixture.Page.Navigate(fixture.Client, "/Home/Database");
        var pageContent = await fixture.Page.ContentAsync();
        await fixture.Page.Screenshot("database");
        Assert.Contains("There are 2 rows in the database.", pageContent);
    }

    [Fact]
    public async Task Can_Make_Third_of_Database()
    {
        Assert.SkipWhen(fixture.Options.Playwright == null, "Playwright disabled.");
        await fixture.Db.Table.ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);
        for (var i = 1; i <= 3; i++)
            await fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            }, TestContext.Current.CancellationToken);
        await fixture.Db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await fixture.Page.Navigate(fixture.Client, "/Home/Database");
        var pageContent = await fixture.Page.ContentAsync();
        await fixture.Page.Screenshot("database");
        Assert.Contains("There are 3 rows in the database.", pageContent);
    }
}
