using Adliance.AspNetCore.Buddy.Testing.DemoProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.DemoProject.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string? postContent)
    {
        await Task.CompletedTask;
        return View(nameof(Index), postContent);
    }

    public async Task<IActionResult> Database([FromServices] DbBase db)
    {
        return View(await db.Table.CountAsync());
    }
}
