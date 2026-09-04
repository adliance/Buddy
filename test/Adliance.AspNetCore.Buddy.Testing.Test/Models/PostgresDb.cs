using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Models;

public class PostgresDb(DbContextOptions<PostgresDb> options) : DbBase(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TableRow>(b => b.HasKey(x => x.Id));
    }
}
