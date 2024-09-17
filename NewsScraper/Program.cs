using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Data.Sqlite;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace NewsScraper;

class Program
{
    public static async Task Main(string[] args)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        Directory.CreateDirectory("articles");
        
        // scrape articles
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
        
        // get user settings from database
        IConfiguration config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        string dbPath = config["DB_PATH"];
        if (string.IsNullOrEmpty(dbPath))
        {
            Console.WriteLine("Database path not configured");
            return;
        }
        List<Settings> settings = GetSettingsList(dbPath);
        Console.WriteLine($"{settings.Count} user settings found");
        
        // generate newspapers
        Directory.CreateDirectory("newspapers");
        QuestPDF.Settings.License = LicenseType.Community;
        foreach (Settings setting in settings)
        {
            var newspaper = new Newspaper(scraper.Articles);
            newspaper.GeneratePdf("newspapers/test.pdf");
        }
        
        timer.Stop(); 
        Console.WriteLine($"Execution time: {timer.Elapsed}");
    }

    public static List<Settings> GetSettingsList(string dbPath)
    {
        List<Settings> settings = new List<Settings>();
        using (SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Settings";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Settings s = new Settings
                    {
                        KindleEmail = reader.GetString(2),
                        IncludeWorld = reader.GetBoolean(3),
                        IncludeUs = reader.GetBoolean(4),
                        IncludePolitics = reader.GetBoolean(5),
                        IncludeBusiness = reader.GetBoolean(6),
                        IncludeSports = reader.GetBoolean(7),
                        IncludeEntertainment = reader.GetBoolean(8),
                        IncludeScience = reader.GetBoolean(9)
                    };
                    settings.Add(s);
                }
            }
        }

        return settings;
    }
}