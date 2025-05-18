using Adliance.AspNetCore.Buddy.Testing.Extensions;
using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class InContainerAdditionalTest(WithDatabaseFixture<InContainerOptions> fixture) : BaseTest<InContainerOptions>(fixture)
{
    private readonly WithDatabaseFixture<InContainerOptions> _fixture = fixture;

    [Fact]
    public async Task Can_Make_Another_Screenshot_of_Database()
    {
        if (_fixture.Options.Playwright == null) return; // Assert.Skip is only available in XUnit 3

        await _fixture.Db.Table.ExecuteDeleteAsync();
        for (var i = 1; i <= 200; i++)
            await _fixture.Db.Table.AddAsync(new TableRow
            {
                Name = "Row " + i
            });
        await _fixture.Db.SaveChangesAsync();

        await _fixture.Page.Navigate(_fixture.Client, "/Home/Database");
        var pageContent = await _fixture.Page.ContentAsync();
        await _fixture.Page.Screenshot("database");
        Assert.Contains("There are 200 rows in the database.", pageContent);
    }
}