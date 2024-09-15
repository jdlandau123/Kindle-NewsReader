using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NewsScraper_Web.Models;
using NewsScraper_Web.Services;

namespace NewsScraper_Web.Controllers;

public class HomeController : Controller
{
    private readonly UserService _userService;

    public HomeController(UserService userService)
    {
        _userService = userService;
    }

    public IActionResult Index()
    {
        bool isLoggedIn = _userService.IsLoggedIn();
        ViewData["IsLoggedIn"] = isLoggedIn;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}