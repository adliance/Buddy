using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class WithDatabaseFixture<TOptions> : BuddyFixture<TOptions, Program>, IDisposable where TOptions : BuddyFixtureOptions<Program>, new()
{
    public Db Db = null!;

    protected override async Task AfterInit()
    {
        Db = new Db(new DbContextOptionsBuilder<Db>().UseSqlServer(Database!.DbConnectionStringExternal).Options);
        await Db.Table.ExecuteDeleteAsync();
    }

    public void Dispose()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        Db?.Dispose();
    }
}
