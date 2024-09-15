using System.ComponentModel.DataAnnotations;

namespace NewsScraper_Web.Models;

public class Settings
{
    [Key]
    public int Id { get; set; }
    
    [EmailAddress]
    public string KindleEmail { get; set; } = string.Empty;
    
    // TODO: categories
}