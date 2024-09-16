using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsScraper_Web.Models;

public class Settings
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [EmailAddress]
    public string KindleEmail { get; set; } = string.Empty;

    public bool IncludeWorld { get; set; } = true;
    public bool IncludeUs { get; set; } = true;
    public bool IncludePolitics { get; set; } = true;
    public bool IncludeBusiness { get; set; } = true;
    public bool IncludeSports { get; set; } = true;
    public bool IncludeEntertainment { get; set; } = true;
    public bool IncludeScience { get; set; } = true;
}