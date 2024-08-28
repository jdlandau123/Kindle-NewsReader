using System.Text;
using ScraperLib;
using Newtonsoft.Json;

namespace NewsScraper;

class Program
{
    private static readonly List<IScraper> _scrapers = 
    [
        new AssociatedPressScraper(),
        new NewYorkTimesScraper(),
        new CnnScraper(),
        new FoxNewsScraper()
    ];
    public static async Task Main(string[] args)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        Directory.CreateDirectory("articles");

        foreach (IScraper scraper in _scrapers)
        {
            string name;
            switch (scraper.GetType().Name)
            {
                case "AssociatedPressScraper":
                    name = "ap";
                    break;
                case "NewYorkTimesScraper":
                    name = "ny-times";
                    break;
                case "CnnScraper":
                    name = "cnn";
                    break;
                case "FoxNewsScraper":
                    name = "fox-news";
                    break;
                default:
                    name = "";
                    break;
            }
            Console.WriteLine($"----- {name} -----");
            List<string> links = await scraper.GetArticleLinks();
            Console.WriteLine($"{links.Count} links found");
            foreach (string link in links)
            {
                await scraper.ScrapeArticle(link);
            }
            Console.WriteLine($"Articles scraped: {scraper.Articles.Count}");
        }
        WriteArticlesToJson();
        timer.Stop();
        Console.WriteLine($"Execution time: {timer.Elapsed}");
    }

    private static void WriteArticlesToJson()
    {
        Directory.CreateDirectory("articles");
        string outFilename = $"articles-{DateTime.Today.ToString("yyyyMMdd")}.json";
        List<Article> articles = [];
        foreach (IScraper scraper in _scrapers)
        {
            articles.AddRange(scraper.Articles);
        }
        string json = JsonConvert.SerializeObject(articles);
        File.WriteAllText(Path.Join("articles", outFilename), json, Encoding.UTF8);
    }
}