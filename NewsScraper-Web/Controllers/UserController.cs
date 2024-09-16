using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsScraper_Web.Models;
using NewsScraper_Web.Services;

namespace NewsScraper_Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly UserService _userService;

        public UserController(ApplicationDbContext context, PasswordService passwordService, UserService userService)
        {
            _context = context;
            _passwordService = passwordService;
            _userService = userService;
        }

        // GET: User
        public IActionResult Index()
        {
            if (_userService.IsLoggedIn())
            {
                return RedirectToAction("Detail", "User");
            }
            return RedirectToAction("Login", "User");
        }

        // GET: User/Detail
        public async Task<IActionResult> Detail()
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Login", "User");
            }

            int userId = _userService.GetLoggedInUserId();

            var user = await _context.Users
                .Include(u => u.Settings)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                Console.WriteLine("User Not Found");
                return RedirectToAction("Login", "User");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(
            [Bind("Id,KindleEmail,IncludeWorld,IncludeUs,IncludePolitics,IncludeBusiness,IncludeSports,IncludeEntertainment,IncludeScience")] Settings settings)
        {
            Settings s = _context.Settings.FirstOrDefault(s => s.Id == settings.Id);
            if (s == null)
            {
                return BadRequest();
            }
            s.KindleEmail = settings.KindleEmail;
            s.IncludeWorld = settings.IncludeWorld;
            s.IncludeUs = settings.IncludeUs;
            s.IncludePolitics = settings.IncludePolitics;
            s.IncludeBusiness = settings.IncludeBusiness;
            s.IncludeSports = settings.IncludeSports;
            s.IncludeEntertainment = settings.IncludeEntertainment;
            s.IncludeScience = settings.IncludeScience;
            await _context.SaveChangesAsync();
            TempData["Message"] = "Changes Saved!";
            return RedirectToAction("Detail", "User");
        }
        
        // GET: User/Register
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            [Bind("Username,Password,ConfirmPassword")] UserRegister userRegister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Form is invalid");
            }

            if (userRegister.Password != userRegister.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }

            var (hash, salt) = _passwordService.HashPassword(userRegister.Password);
            
            User user = new User
            {
                Username = userRegister.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Settings = new Settings()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _userService.Login(user);
            return RedirectToAction("Detail", "User");
        }
        
        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] UserLogin login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Form is invalid");
            }
            
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == login.Username);
            if (user == null || !_passwordService.VerifyPasswordHash(
                    login.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Username or password is incorrect");
            }
            
            await _userService.Login(user);
            return RedirectToAction("Detail", "User");
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        

        // GET: User/Delete
        public async Task<IActionResult> Delete()
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Login", "User");
            }

            int userId = _userService.GetLoggedInUserId();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(user);
        }

        // POST: User/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Login", "User");
            }

            int userId = _userService.GetLoggedInUserId();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
