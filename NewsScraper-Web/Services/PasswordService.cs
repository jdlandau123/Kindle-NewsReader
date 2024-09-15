using System.Security.Cryptography;

namespace NewsScraper_Web.Services;

public class PasswordService
{
    public (byte[], byte[]) HashPassword(string password)
    {
        using (var hmac = new HMACSHA512())
        {
            byte[] passwordSalt = hmac.Key;
            byte[] passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return (passwordHash, passwordSalt);
        }
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return hash.SequenceEqual(passwordHash);
        }
    }
}