using System.ComponentModel.DataAnnotations;

namespace NewsScraper_Web.Models;

public class UserLogin
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
}