using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace NewsScraper_Web.Models;

public class Contact
{
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    public string Message { get; set; }

    public void SendEmail(IConfiguration config, User user)
    {
        string apiKey = config["SMTP_API_KEY"];
        if (apiKey == null)
        {
            Console.WriteLine("SMTP server not configured");
            return;
        }
        
        string adminEmail = config["ADMIN_EMAIL"];
        if (adminEmail == null)
        {
            Console.WriteLine("Admin email not configured");
            return;
        }

        if (Configuration.Default.ApiKey.Values.IsNullOrEmpty())
        {
            Configuration.Default.ApiKey.Add("api-key", apiKey);
        }
        var emailService = new TransactionalEmailsApi();
        SendSmtpEmailSender sender = new SendSmtpEmailSender("Kindle NewsReader", "kindlenewsreader@noreply.com");
        SendSmtpEmailTo toEmail = new SendSmtpEmailTo(adminEmail);
        List<SendSmtpEmailTo> to = [ toEmail ];
        string htmlContent = $"<p>From: {Email ?? "No email given"}</p><p>{Message}</p>";
        string subject = "Message received from Kindle NewsReader";
        SendSmtpEmail mail = new();
        mail.Sender = sender;
        mail.To = to;
        mail.HtmlContent = htmlContent;
        mail.Subject = subject;
        emailService.SendTransacEmail(mail);
    }
}