using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace NewsScraper;

class Program
{
    public static IConfiguration Config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    public static async System.Threading.Tasks.Task Main(string[] args)
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
        // WriteArticlesToJson(scraper.Articles);
        
        // get user settings from database
        string dbPath = Config["DB_PATH"];
        if (string.IsNullOrEmpty(dbPath))
        {
            throw new Exception("Database path not configured");
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
            SendEmail(setting.KindleEmail, filepath);
        }
        
        timer.Stop();
        Console.WriteLine($"Execution time: {timer.Elapsed}");
    }

    // public static void WriteArticlesToJson(List<Article> articles)
    // {
    //     Directory.CreateDirectory("articles");
    //     string outFilename = $"articles-{DateTime.Today.ToString("yyyyMMdd")}.json";
    //     string json = JsonConvert.SerializeObject(articles);
    //     File.WriteAllText(Path.Join("articles", outFilename), json, Encoding.UTF8);
    // }

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

    public static void SendEmail(string email, string pdfPath)
    {
        string apiKey = Config["SMTP_API_KEY"];
        if (apiKey == null)
        {
            Console.WriteLine("SMTP server not configured");
            return;
        }

        Configuration.Default.ApiKey.Add("api-key", apiKey);
        var emailService = new TransactionalEmailsApi();
        SendSmtpEmailSender sender = new SendSmtpEmailSender("Kindle NewsReader", "kindlenewsreader@noreply.com");
        SendSmtpEmailTo toEmail = new SendSmtpEmailTo(email);
        List<SendSmtpEmailTo> to = [ toEmail ];
        string textContent = "Here's your daily news digest from Kindle NewsReader";
        string subject = $"Kindle NewsReader {DateTime.Today.ToString("d")}";
        byte[] content = File.ReadAllBytes(pdfPath);
        string attachmentName = pdfPath.Split("/").Last();
        SendSmtpEmailAttachment attachmentContent = new SendSmtpEmailAttachment(null, content, attachmentName);
        List<SendSmtpEmailAttachment> attachment = [ attachmentContent ];
        SendSmtpEmail mail = new();
        mail.Sender = sender;
        mail.To = to;
        mail.TextContent = textContent;
        mail.Subject = subject;
        mail.Attachment = attachment;
        emailService.SendTransacEmail(mail);
    }
}
