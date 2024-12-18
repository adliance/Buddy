using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Models;

public class Db(DbContextOptions options) : DbContext(options)
{
    public DbSet<TableRow> Table => Set<TableRow>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TableRow>(b =>
        {
            b.HasKey(x => x.Id);
            b.ToTable(x => x.IsTemporal());
        });
    }
}
