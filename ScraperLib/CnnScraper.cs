using AngleSharp.Dom;
using AngleSharp;

namespace ScraperLib;

public class CnnScraper : BaseScraper, IScraper
{
    private string _urlRoot = "https://www.cnn.com";

    public async Task<List<string>> GetArticleLinks()
    {
        List<string> articleLinks = new();
        IDocument document = await _browsingContext.OpenAsync(_urlRoot);
        IHtmlCollection<IElement> links = document.QuerySelectorAll("a[data-link-type='article']")
            .ToCollection();
        foreach (IElement link in links)
        {
            string href = link.GetAttribute("href");
            if (href != null && !articleLinks.Contains(href) && articleLinks.Count < _maxArticles)
            {
                articleLinks.Add(href);
            }
        }
        return articleLinks;
    }

    public async Task ScrapeArticle(string url)
    {
        url = $"{_urlRoot}{url}";
        IDocument document = await _browsingContext.OpenAsync(url);
        IEnumerable<string> paragraphs = document.QuerySelectorAll("div[itemprop='articleBody'] > p")
            .Select(p => p.TextContent.Trim());
        string title = document.QuerySelector("h1.headline__text")?.TextContent.Trim();
        string[] urlFragments = url.Split("/").Skip(3).ToArray();
        if (urlFragments[0] != DateTime.Today.Year.ToString())
        {
            return;
        }
        string category = urlFragments[3];
        if (new List<string>{ "us", "world" }.Contains(category))
        {
            category = $"{urlFragments[3]}-{urlFragments[4]}";
        }
        DateTime date = DateTime.Parse($"{urlFragments[0]}/{urlFragments[1]}/{urlFragments[2]}");
        Article article = new()
        {
            Title = title != null ? title.Replace('\u00A0', ' ') : "",
            Category = category,
            UpdatedDate = date,
            Link = url,
            Body = String.Join(" ", paragraphs).Replace('\u00A0', ' ')
        };
        Articles.Add(article);
    }
}