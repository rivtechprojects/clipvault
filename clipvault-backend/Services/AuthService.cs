using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ClipVault.Models;
using Microsoft.EntityFrameworkCore;

namespace clipvault_backend.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(AppDbContext context, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> RegisterUserAsync(string username, string email, string password)
    {
        if (_context.Users.Any(u => u.UserName == username || u.Email == email))
            throw new Exception("Username or email already exists.");

        var tempUser = new User
        {
            UserName = username,
            Email = email,
            PasswordHash = "temp", // Will be replaced below
            CreatedAt = DateTime.UtcNow
        };
        tempUser.PasswordHash = _passwordHasher.HashPassword(tempUser, password);
        _context.Users.Add(tempUser);
        await _context.SaveChangesAsync();
        return tempUser;
    }

    public async Task<string> LoginUserAsync(string usernameOrEmail, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);
        if (user == null)
            throw new Exception("Invalid username or password.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Invalid username or password.");

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "your_default_secret";
        var issuer = jwtSettings["Issuer"] ?? "ClipVault";
        var audience = jwtSettings["Audience"] ?? "ClipVaultUsers";
        var expires = DateTime.UtcNow.AddHours(2);

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

