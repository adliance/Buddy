var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseRouting();
app.MapDefaultControllerRoute();
app.Run();

public partial class Program { } // required for integration testing to make it visible to WebApplicationFactory