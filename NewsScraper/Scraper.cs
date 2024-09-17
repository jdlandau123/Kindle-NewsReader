using AngleSharp;
using AngleSharp.Dom;

namespace NewsScraper;

public class Scraper
{
    public List<Article> Articles { get; } = new();
    
    private readonly string _urlRoot = "https://apnews.com";

    private IBrowsingContext _browsingContext { get; } =
        BrowsingContext.New(Configuration.Default.WithDefaultLoader());

    private readonly int _maxArticles = 50;
    
    public async Task<List<string>> GetArticleLinks()
    {
        List<string> articleLinks = new();
        IDocument document = await _browsingContext.OpenAsync(_urlRoot);
        IHtmlCollection<IElement> links = document.QuerySelectorAll("a[href]")
            .Where(a => a.GetAttribute("href").Contains($"{_urlRoot}/article/"))
            .ToCollection();
        foreach (IElement link in links)
        {
            string href = link.GetAttribute("href");
            if (!articleLinks.Contains(href) && articleLinks.Count < _maxArticles)
            {
                articleLinks.Add(href);
            }
        }
        return articleLinks;
    }

    public async Task ScrapeArticle(string url)
    {
        IDocument document = await _browsingContext.OpenAsync(url);
        IEnumerable<string> paragraphs = document.QuerySelectorAll("div.RichTextBody > p")
            .Select(p => p.TextContent.Trim());
        string title = document.QuerySelector("h1.Page-headline").TextContent.Trim();
        string category = document.QuerySelector("div.Page-breadcrumbs > a").TextContent.Trim();
        string timestamp = document.QuerySelector("div.Page-dateModified>bsp-timestamp")
            .GetAttribute("data-timestamp");
        DateTime date = TimestampToDateTime(timestamp);
        Article article = new()
        {
            Title = title,
            Category = category,
            UpdatedDate = date,
            Link = url,
            Body = String.Join(" ", paragraphs)
        };
        if (article.IsValid())
        {
            Articles.Add(article);
        }
    }

    public DateTime TimestampToDateTime(string timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(Convert.ToDouble(timestamp) / 1000).ToLocalTime();
        return dateTime;
    }
}