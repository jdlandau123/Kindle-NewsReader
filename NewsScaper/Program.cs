using ScraperLib;

namespace NewsScraper;

class Program
{
    public static async Task Main(string[] args)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        
        List<IScraper> scrapers = new List<IScraper>
        {
            // new AssociatedPressScraper(),
            // new NewYorkTimesScraper(),
            new CnnScraper()
        };

        foreach (IScraper scraper in scrapers)
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
                default:
                    name = "";
                    break;
            }
            List<string> links = await scraper.GetArticleLinks();
            Console.WriteLine($"{links.Count} links found");
            foreach (string link in links)
            {
                await scraper.ScrapeArticle(link);
            }
            Console.WriteLine($"Articles scraped: {scraper.Articles.Count}");
            scraper.WriteArticlesToJson($"{name}-articles.json");
        }
        
        timer.Stop();
        Console.WriteLine($"Execution time: {timer.Elapsed}");
    }
}