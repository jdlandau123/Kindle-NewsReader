namespace NewsScraper;

public class Article
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; } = DateTime.Today;

    public bool IsValid()
    {
        string[] requiredProperties =
        [
            Title,
            Body,
            Link,
            UpdatedDate.ToString("yyyy-MM-dd"),
        ];
        return !requiredProperties.Any(p => string.IsNullOrWhiteSpace(p));
    }
}