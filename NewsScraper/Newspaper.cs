using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NewsScraper;

public class Newspaper : IDocument
{
    public List<Article> Articles { get; } 

    public Newspaper(List<Article> articles)
    {
        Articles = articles;
    }
    
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(15);
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
        });
    }
    
    void ComposeHeader(IContainer container)
    {
        var titleStyle = TextStyle.Default.FontSize(28).SemiBold().FontColor(Colors.Black);
    
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item()
                    .ShowOnce()
                    .Text($"Kindle NewsReader Digest - {DateTime.Today.ToString("D")}")
                    .Style(titleStyle)
                    .AlignCenter();
            });
        });
    }
    
    void ComposeContent(IContainer container)
    {
        container
            .PaddingTop(40)
            .AlignCenter()
            .Column(column =>
            {
                column.Spacing(25);
                foreach (Article article in Articles)
                {
                    column.Item().Text(text =>
                    {
                        text.Span(article.Title).FontSize(24).LineHeight(1.5f).SemiBold();
                        text.EmptyLine();
                        text.Span(article.Category).FontSize(20).LineHeight(1.5f);
                        text.EmptyLine();
                        text.Span($"Last updated: {article.UpdatedDate.ToString("d")}")
                            .FontSize(20).LineHeight(1.5f);
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span(article.Body).FontSize(20).LineHeight(1.5f);
                    });
                    column.Item().PaddingHorizontal(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                }
            });
    }
}