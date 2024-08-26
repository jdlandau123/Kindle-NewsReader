using AngleSharp.Dom;
using AngleSharp;

namespace ScraperLib;

public class NewYorkTimesScraper : BaseScraper, IScraper
{
    private string _urlRoot = "https://www.nytimes.com";
    
    public async Task<List<string>> GetArticleLinks()
    {
        List<string> articleLinks = new();
        IDocument document = await _browsingContext.OpenAsync(_urlRoot);
        IHtmlCollection<IElement> links = document.QuerySelectorAll("a[data-uri]")
            .Where(a => a.GetAttribute("data-uri").Contains("nyt://article/"))
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
        IEnumerable<string> paragraphs = document.QuerySelectorAll("div.StoryBodyCompanionColumn > * > p")
            .Select(p => p.TextContent.Trim());
        string title = document.QuerySelector("h1[data-testid='headline']").TextContent.Trim();
        string[] urlFragments = url.Split("/").Skip(3).ToArray();
        string category = urlFragments[3];
        if (new List<string>{ "us", "world" }.Contains(category))
        {
            category = $"{urlFragments[3]}-{urlFragments[4]}";
        }
        DateTime date = DateTime.Parse($"{urlFragments[0]}/{urlFragments[1]}/{urlFragments[2]}");
        Article article = new()
        {
            Title = title,
            Category = category,
            UpdatedDate = date,
            Link = url,
            Body = String.Join(" ", paragraphs)
        };
        Articles.Add(article);
    }
}