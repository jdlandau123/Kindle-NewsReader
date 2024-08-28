using AngleSharp;

namespace ScraperLib;

public abstract class BaseScraper
{
    public List<Article> Articles { get; } = new();

    protected IBrowsingContext _browsingContext { get; } =
        BrowsingContext.New(Configuration.Default.WithDefaultLoader());

    protected int _maxArticles = 5;

    public DateTime TimestampToDateTime(string timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(Convert.ToDouble(timestamp) / 1000).ToLocalTime();
        return dateTime;
    }
}