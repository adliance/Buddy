using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.DemoProject.Models;

public class SqlServerDb(DbContextOptions<SqlServerDb> options) : DbBase(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TableRow>(b =>
        {
            b.HasKey(x => x.Id);
            b.ToTable(x => x.IsTemporal());
        });
    }
}