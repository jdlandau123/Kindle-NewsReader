using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
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
            });

        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<UserService>();
        
        builder.Services.AddHttpContextAccessor();
        
        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
        });

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseSession();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}