using Microsoft.AspNetCore.Mvc;

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
}
