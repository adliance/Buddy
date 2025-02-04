using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class WithDatabaseFixture<TOptions> : BuddyFixture<TOptions, Program>, IDisposable where TOptions : IFixtureOptions, new()
{
    public Db Db = null!;

    protected override async Task AfterInit()
    {
        Db = new Db(new DbContextOptionsBuilder<Db>().UseSqlServer(DbConnectionStringExternal).Options);
        await Db.Table.ExecuteDeleteAsync();
    }

    public void Dispose()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        Db?.Dispose();
    }
}
