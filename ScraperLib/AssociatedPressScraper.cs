using AngleSharp.Dom;
using AngleSharp;

namespace ScraperLib;

public class AssociatedPressScraper : BaseScraper, IScraper
{
    private string _urlRoot = "https://apnews.com";

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
            Body = String.Join(" ", paragraphs),
            Source = "Associated Press"
        };
        if (article.IsValid())
        {
            Articles.Add(article);
        }
    }
}