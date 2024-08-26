namespace ScraperLib;

public class Article
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
    // public string ImageSrc { get; set; }
    public string Link { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; } = DateTime.Today;
}