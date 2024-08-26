namespace ScraperLib;

public interface IScraper
{
    public List<Article> Articles { get; }
    public Task<List<string>> GetArticleLinks();
    public Task ScrapeArticle(string url);
    public void WriteArticlesToJson(string filename);
}