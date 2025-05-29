using System.Security.Cryptography;
using System.Text;
using ClipVault.Interfaces;

namespace ClipVault.Services;

public class SHA256HashingService : IHashingService
{
    public string Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashedBytes);
    }
}