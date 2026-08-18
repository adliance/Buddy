using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Models;

public abstract class DbBase(DbContextOptions options) : DbContext(options)
{
    public DbSet<TableRow> Table => Set<TableRow>();
}