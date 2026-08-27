using Adliance.AspNetCore.Buddy.Testing.Shared.Database;
using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SqlServerDb>(options =>
{
    var connectionString = builder.Configuration.GetValue<string>("DatabaseConnectionString");
    options.UseSqlServer(connectionString);
});
builder.Services.AddDbContext<PostgresDb>(options =>
{
    var connectionString = builder.Configuration.GetValue<string>("DatabaseConnectionString");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<DbBase>(sp =>
{
    var databaseType = sp.GetRequiredService<IConfiguration>().GetValue<DatabaseType>("DatabaseType");
    return databaseType is DatabaseType.UsePostgresContainer or DatabaseType.UsePostgresLocal
        ? sp.GetRequiredService<PostgresDb>()
        : sp.GetRequiredService<SqlServerDb>();
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

var connectionString = app.Configuration.GetValue<string>("DatabaseConnectionString");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    await using (var scope = app.Services.CreateAsyncScope())
    await using (var db = scope.ServiceProvider.GetRequiredService<DbBase>())
    {
        try
        {
            await db.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while migrate the database at {connectionString}: {ex.Message}");
        }
    }
}

app.UseRouting();
app.MapDefaultControllerRoute();
app.Run();

public partial class Program
{
} // required for integration testing to make it visible to WebApplicationFactory