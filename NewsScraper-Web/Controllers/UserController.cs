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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                Console.WriteLine("User Not Found");
                return RedirectToAction("Login", "User");
            }

            return View(user);
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
            Settings settings = new Settings();
            
            User user = new User
            {
                Username = userRegister.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Settings = settings,
                SettingsId = settings.Id
            };

            await _userService.Login(user);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
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

            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //     new Claim(ClaimTypes.Name, user.Username)
            // };
            //
            // var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // var authProperties = new AuthenticationProperties();
            //
            // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            //     new ClaimsPrincipal(claimsIdentity), authProperties);
            
            await _userService.Login(user);
            
            // TODO: Send to settings page here
            return RedirectToAction("Index", "Home");
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        

        // GET: User/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var user = await _context.Users.FindAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(user);
        // }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("Id,Username,PasswordHash,PasswordSalt,KindleEmail")] User user)
        // {
        //     if (id != user.Id)
        //     {
        //         return NotFound();
        //     }
        //
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(user);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!UserExists(user.Id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(user);
        // }

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
