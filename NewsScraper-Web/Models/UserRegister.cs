using System.ComponentModel.DataAnnotations;

namespace NewsScraper_Web.Models;

public class UserRegister
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    [Required]
    public string ConfirmPassword { get; set; }
}