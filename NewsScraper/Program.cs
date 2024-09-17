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
        
        // scrape articles
        Scraper scraper = new Scraper();
        List<string> links = await scraper.GetArticleLinks();
        Console.WriteLine($"Article links found: {links.Count}");
        foreach (string link in links)
        {
            await scraper.ScrapeArticle(link);
        }
        Console.WriteLine($"Articles scraped: {scraper.Articles.Count}");
        
        // write to json file - REMOVE?
        WriteArticlesToJson(scraper.Articles);
        
        // get user settings from database
        IConfiguration config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        string dbPath = config["DB_PATH"];
        if (string.IsNullOrEmpty(dbPath))
        {
            Console.WriteLine("Database path not configured");
            return;
        }
        List<Settings> settings = GetSettingsList(dbPath);
        Console.WriteLine($"User configs found: {settings.Count}");
        
        // generate newspapers
        Directory.CreateDirectory("newspapers");
        QuestPDF.Settings.License = LicenseType.Community; 
        foreach (Settings setting in settings)
        {
            List<Article> articles = setting.GetFilteredArticles(scraper.Articles);
            Newspaper newspaper = new Newspaper(articles);
            string filepath = 
                $"newspapers/KindleNewsReader_{setting.Username}_{DateTime.Today.ToString("yyyyMMdd")}.pdf";
            newspaper.GeneratePdf(filepath);
        }
        
        timer.Stop(); 
        Console.WriteLine($"Execution time: {timer.Elapsed}");
    }

    public static void WriteArticlesToJson(List<Article> articles)
    {
        Directory.CreateDirectory("articles");
        string outFilename = $"articles-{DateTime.Today.ToString("yyyyMMdd")}.json";
        string json = JsonConvert.SerializeObject(articles);
        File.WriteAllText(Path.Join("articles", outFilename), json, Encoding.UTF8);
    }

    public static List<Settings> GetSettingsList(string dbPath)
    {
        List<Settings> settings = new List<Settings>();
        using (SqliteConnection connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT KindleEmail, IncludeWorld, IncludeUs, IncludePolitics, IncludeBusiness, 
                                            IncludeSports, IncludeEntertainment, IncludeScience, Users.Username 
                                    FROM Settings INNER JOIN Users ON Settings.UserId = Users.Id
                                    WHERE KindleEmail IS NOT NULL";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Settings s = new Settings
                    {
                        KindleEmail = reader.GetString(0),
                        IncludeWorld = reader.GetBoolean(1),
                        IncludeUs = reader.GetBoolean(2),
                        IncludePolitics = reader.GetBoolean(3),
                        IncludeBusiness = reader.GetBoolean(4),
                        IncludeSports = reader.GetBoolean(5),
                        IncludeEntertainment = reader.GetBoolean(6),
                        IncludeScience = reader.GetBoolean(7),
                        Username = reader.GetString(8)
                    };
                    settings.Add(s);
                }
            }
        }
        
        return settings;
    }
}
