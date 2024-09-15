using System.ComponentModel.DataAnnotations;

namespace NewsScraper_Web.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    public byte[] PasswordHash { get; set; }
    
    [Required]
    public byte[] PasswordSalt { get; set; }

    [Required]
    public Settings Settings { get; set; }
    
    [Required]
    public int SettingsId { get; set; }
}