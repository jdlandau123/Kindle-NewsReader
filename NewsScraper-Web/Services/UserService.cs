using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NewsScraper_Web.Models;

namespace NewsScraper_Web.Services;

public class UserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Login(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();

        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);
    }
    public bool IsLoggedIn()
    {
        string userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return !string.IsNullOrEmpty(userid);
    }

    public int GetLoggedInUserId()
    {
        string userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userid))
        {
            return 0;
        }
        return int.Parse(userid);
    }

    public string GetLoggedInUserName()
    {
        string username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return "";
        }
        return username;
    }

    public void LogUser()
    {
        string username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        string userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"Userid: {userid}, Username: {username}");
    }
}