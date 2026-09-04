using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Models;

public class SqlServerDbFactory : IDesignTimeDbContextFactory<SqlServerDb>
{
    public SqlServerDb CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SqlServerDb>()
            .UseSqlServer("server=localhost;user id=sa;password=Passw0rd!;database=db;encrypt=false;")
            .Options;
        return new SqlServerDb(options);
    }
}
