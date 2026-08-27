using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Adliance.AspNetCore.Buddy.Testing.DemoProject.Models;

public class PostgresDbFactory : IDesignTimeDbContextFactory<PostgresDb>
{
    public PostgresDb CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PostgresDb>()
            .UseNpgsql("Host=localhost;Database=db;Username=postgres;Password=postgres")
            .Options;
        return new PostgresDb(options);
    }
}