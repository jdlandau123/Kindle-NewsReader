using AngleSharp;
using AngleSharp.Dom;

namespace ScraperLib;

public class FoxNewsScraper : BaseScraper, IScraper
{
    private string _urlRoot = "https://www.foxnews.com/";

    public async Task<List<string>> GetArticleLinks()
    {
        List<string> articleLinks = new();
        IDocument document = await _browsingContext.OpenAsync(_urlRoot);
        IHtmlCollection<IElement> links = document.QuerySelectorAll("article > * > a[href]")
            .Where(a => a.GetAttribute("href").Contains(_urlRoot))
            .Where(a => !a.GetAttribute("href").Contains("video"))
            .Where(a => !a.GetAttribute("href").Contains("shows"))
            .Where(a => a.GetAttribute("href").Split("/").Length > 4)
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
        Console.WriteLine(url);
        IDocument document = await _browsingContext.OpenAsync(url);
        // document = WebUtility.HtmlDecode(document.ToHtml());
        IEnumerable<string> paragraphs = document.QuerySelectorAll("div.article-body > p")
            .Select(p => p.TextContent.Trim());
        string title = document.QuerySelector("h1.headline")?.TextContent.Trim();
        string[] urlFragments = url.Split("/").Skip(3).ToArray();
        string category = urlFragments.First();
        string dateString = document.QuerySelector("span.article-date > time")?.TextContent.Trim();
        if (dateString != null)
        {
            dateString = String.Join(" ", dateString.Split(" ").Take(3));
        }
        Article article = new()
        {
            Title = title != null ? title : "",
            Category = category,
            UpdatedDate = dateString != null ? DateTime.Parse(dateString) : DateTime.Today,
            Link = url,
            Body = String.Join(" ", paragraphs),
            Source = "Fox News"
        };
        if (article.IsValid())
        {
            Articles.Add(article);
        }
    }
}