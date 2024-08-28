namespace ScraperLib;

public class Article
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; } = DateTime.Today;
    public string Source { get; set; } = string.Empty;

    public bool IsValid()
    {
        string[] requiredProperties =
        [
            Title,
            Body,
            Link,
            Source,
            UpdatedDate.ToString("yyyy-MM-dd"),
        ];
        return !requiredProperties.Any(p => string.IsNullOrWhiteSpace(p));
    }
}