using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NewsScraper_Web.Models;
using NewsScraper_Web.Services;

namespace NewsScraper_Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=app.db"));
        
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                // options.LoginPath = "/api/Account/login";
                // options.LogoutPath = "/api/Account/logout";
                // options.AccessDeniedPath = "/api/Account/access-denied";
                // options.Events.OnRedirectToAccessDenied = context =>
                // {
                //     context.Response.ContentType = "application/json";
                //     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //     return context.Response.WriteAsync("{\"message\": \"Access denied.\"}");
                // };
                // options.Events.OnRedirectToLogin = context =>
                // {
                //     context.Response.ContentType = "application/json";
                //     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //     return context.Response.WriteAsync("{\"message\": \"Unauthorized access.\"}");
                // };
            });

        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddHttpContextAccessor();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}