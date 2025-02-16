using Adliance.AspNetCore.Buddy.Testing.Extensions;
using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InContainerAdditionalTest(WithDatabaseFixture<InContainerOptions> fixture) : BaseTest<InContainerOptions>(fixture)
{
    [Fact]
    public async Task Can_Make_Another_Screenshot_of_Database()
    {
        if (fixture.Options.Playwright == null) return; // Assert.Skip is only available in XUnit 3

        await fixture.Db.Table.ExecuteDeleteAsync();
        for (var i = 1; i <= 200; i++)
            await fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            });
        await fixture.Db.SaveChangesAsync();

        await fixture.Page.Navigate(fixture.Client, "/Home/Database");
        var pageContent = await fixture.Page.ContentAsync();
        await fixture.Page.Screenshot("database");
        Assert.Contains("There are 200 rows in the database.", pageContent);
    }
}