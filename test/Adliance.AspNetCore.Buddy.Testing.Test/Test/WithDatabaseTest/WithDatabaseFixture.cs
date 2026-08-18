using Adliance.AspNetCore.Buddy.Testing.Shared;
using Adliance.AspNetCore.Buddy.Testing.Shared.Database;
using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithDatabaseTest;

public class WithDatabaseFixture<TOptions>
    : BuddyFixture<TOptions, Program>, IDisposable
    where TOptions : BuddyFixtureOptions<Program>, new()
{
    public DbBase Db = null!;

    protected override async Task AfterInit()
    {
        Db = Options.Database!.Type is DatabaseType.UsePostgresContainer or DatabaseType.UsePostgresLocal
            ? new PostgresDb(new DbContextOptionsBuilder<PostgresDb>().UseNpgsql(Database!.DbConnectionStringExternal).Options)
            : new SqlServerDb(new DbContextOptionsBuilder<SqlServerDb>().UseSqlServer(Database!.DbConnectionStringExternal).Options);

        await Db.Table.ExecuteDeleteAsync();
    }

    public void Dispose()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        Db?.Dispose();
    }
}