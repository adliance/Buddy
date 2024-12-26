using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Db>(options =>
{
    var connectionString = builder.Configuration.GetValue<string>("DatabaseConnectionString");
    options.UseSqlServer(connectionString);
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

var connectionString = app.Configuration.GetValue<string>("DatabaseConnectionString");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    await using (var scope = app.Services.CreateAsyncScope())
    await using (var db = scope.ServiceProvider.GetRequiredService<Db>())
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
