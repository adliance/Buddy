using Adliance.AspNetCore.Buddy.Testing.Test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Controllers;

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

    public async Task<IActionResult> Database([FromServices] Db db)
    {
        return View(await db.Table.CountAsync());
    }
}
