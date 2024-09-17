namespace NewsScraper;

public class Settings
{
    public string Username { get; set; }
    public string KindleEmail { get; set; }
    public bool IncludeWorld { get; set; }
    public bool IncludeUs { get; set; }
    public bool IncludePolitics { get; set; }
    public bool IncludeBusiness { get; set; }
    public bool IncludeSports { get; set; } 
    public bool IncludeEntertainment { get; set; }
    public bool IncludeScience { get; set; }

    public List<Article> GetFilteredArticles(List<Article> articles)
    {
        Dictionary<string, bool> filtersMap = new Dictionary<string, bool>
        {
            { "World News", IncludeWorld },
            { "U.S. News", IncludeUs },
            { "Politics", IncludePolitics },
            { "Business", IncludeBusiness },
            { "Sports", IncludeSports },
            { "Entertainment", IncludeEntertainment },
            { "Science", IncludeScience}
        };
        
        if (filtersMap.Values.All(c => c))
        {
            return articles; // return all articles if all categories are included
        }

        List<string> allowedCategories = new List<string>();
        foreach (KeyValuePair<string, bool> filter in filtersMap)
        {
            if (filter.Value)
            {
                allowedCategories.Add(filter.Key);
            }
        }
        
        List<Article> filteredArticles = articles
            .Where(a => allowedCategories.Contains(a.Category))
            .ToList();
        return filteredArticles;
    }
}