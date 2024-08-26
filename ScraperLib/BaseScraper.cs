using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using AngleSharp;

namespace ScraperLib;

public abstract class BaseScraper
{
    public List<Article> Articles { get; } = new List<Article>();
    protected IBrowsingContext _browsingContext { get; } = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
    
    protected int _maxArticles = 50;
    
    public DateTime TimestampToDateTime(string timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(Convert.ToDouble(timestamp)/1000).ToLocalTime();
        return dateTime;
    }

    public void WriteArticlesToJson(string filename)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.GeneralPunctuation),
            // Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true,
        };
        string json = JsonSerializer.Serialize(Articles, options);
        File.WriteAllText(filename, json, Encoding.UTF8);
    }
}