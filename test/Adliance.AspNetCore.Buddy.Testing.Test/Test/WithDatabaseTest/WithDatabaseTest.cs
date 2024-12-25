using Adliance.AspNetCore.Buddy.Testing.Extensions;
using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

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
    public async Task Can_Get_Database()
    {
        await using var db = PrepareDatabase();
        for (var i = 1; i <= 100; i++) await db.Table.AddAsync(new TableRow
        {
            Name = "Row " + i
        });
        await db.SaveChangesAsync();
        
        var response = await Client.GetAsync("/Home/Database");
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Assert.Contains("There are 100 rows in the database.", responseString);
    }
    
    [Fact]
    public async Task Can_Make_Screenshot_of_Database()
    {
        if (Options.Playwright == PlaywrightOptions.None) return; // Assert.Skip is only available in XUnit 3
        
        await using var db = PrepareDatabase();
        for (var i = 1; i <= 50; i++) await db.Table.AddAsync(new TableRow
        {
            Name = "Row " + i
        });
        await db.SaveChangesAsync();
        
        await Page.Navigate(Client, "/Home/Database");
        var pageContent = await Page.ContentAsync();
        await Page.Screenshot("database");
        Assert.Contains("There are 50 rows in the database.", pageContent);
    }

    private Db PrepareDatabase()
    {
        var db = new Db(new DbContextOptionsBuilder<Db>().UseSqlServer(DbConnectionStringExternal).Options);
        db.Table.ExecuteDelete();
        return db;
    }
}