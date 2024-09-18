using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsScraper_Web.Models;
using NewsScraper_Web.Services;

namespace NewsScraper_Web.Controllers;

public class HomeController : Controller
{
    private readonly UserService _userService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public HomeController(UserService userService, ApplicationDbContext context, IConfiguration config)
    {
        _userService = userService;
        _context = context;
        _config = config;
    }

    public IActionResult Index()
    {
        bool isLoggedIn = _userService.IsLoggedIn();
        ViewData["IsLoggedIn"] = isLoggedIn;
        return View();
    }

    public IActionResult About()
    {
        bool isLoggedIn = _userService.IsLoggedIn();
        ViewData["IsLoggedIn"] = isLoggedIn;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> About([Bind("Email,Message")] Contact contact)
    {
        bool isLoggedIn = _userService.IsLoggedIn();
        ViewData["IsLoggedIn"] = isLoggedIn;
        
        if (!ModelState.IsValid)
        {
            return View();
        }

        if (!isLoggedIn)
        {
            return RedirectToAction("Help", "Home");
        }
        int userId = _userService.GetLoggedInUserId();
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        contact.SendEmail(_config, user);
        TempData["Message"] = "Message sent successfully!";
        return View();
    }

    public IActionResult Help()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}