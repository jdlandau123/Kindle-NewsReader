using System.Security.Claims;
using NewsScraper_Web.Models;

namespace NewsScraper_Web.Services;

public class UserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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

    public void LogUser()
    {
        string username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        string userid = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"Userid: {userid}, Username: {username}");
    }
}