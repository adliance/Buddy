using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.DemoProject.Models;

public abstract class DbBase(DbContextOptions options) : DbContext(options)
{
    public DbSet<TableRow> Table => Set<TableRow>();
}