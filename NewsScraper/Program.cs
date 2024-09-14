using System.Text;
using Newtonsoft.Json;

namespace NewsScraper;

class Program
{
    public static async Task Main(string[] args)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        Directory.CreateDirectory("articles");
        
        Scraper scraper = new Scraper();
        List<string> links = await scraper.GetArticleLinks();
        Console.WriteLine($"{links.Count} links found");
        foreach (string link in links)
        {
            Console.WriteLine(link);
            await scraper.ScrapeArticle(link);
        }
        Console.WriteLine($"Articles scraped: {scraper.Articles.Count}");
        
        string outFilename = $"articles-{DateTime.Today.ToString("yyyyMMdd")}.json";
        string json = JsonConvert.SerializeObject(scraper.Articles);
        File.WriteAllText(Path.Join("articles", outFilename), json, Encoding.UTF8);
        
        timer.Stop(); 
        Console.WriteLine($"Execution time: {timer.Elapsed}");
    }
}