using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ClipVault.Models;
using Microsoft.EntityFrameworkCore;
using ClipVault.Interfaces;
using ClipVault.Utils;
using ClipVault.Exceptions;
using System.Security.Cryptography;

namespace ClipVault.Services;

public class AuthService : IAuthService
{
    private const int DefaultTokenExpirationMinutes = 120;

    private readonly IAppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IHashingService _hashingService;

    public AuthService(IAppDbContext context, IConfiguration configuration, IPasswordHasher<User> passwordHasher, IHashingService hashingService)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _hashingService = hashingService;
    }

    public async Task<User> RegisterUserAsync(string username, string email, string password)
    {
        try
        {
            if (_context.Users.Any(u => u.UserName == username || u.Email == email))
                throw new UserAlreadyExistsException("Username or email already exists.");

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
        catch (UserAlreadyExistsException)
        {
            throw; // Rethrow the specific exception
        }
        catch (Exception ex)
        {
            throw new RegistrationFailedException("An unexpected error occurred during registration.", ex);
        }
    }

    public async Task<string> LoginUserAsync(string usernameOrEmail, string password)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);
            if (user == null)
                throw new InvalidCredentialsException("Invalid username or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                throw new InvalidCredentialsException("Invalid username or password.");

            return GenerateJwtToken(user);
        }
        catch (InvalidCredentialsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Wrap other exceptions in LoginFailedException
            throw new LoginFailedException("An unexpected error occurred during login.", ex);
        }
    }

    public async Task<(string AccessToken, string RefreshToken)> LoginUserWithRefreshTokenAsync(string usernameOrEmail, string password)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);
            if (user == null)
                throw new InvalidCredentialsException("Invalid username or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
                throw new InvalidCredentialsException("Invalid username or password.");

            var refreshToken = GenerateRefreshToken();
            await UpdateRefreshTokenAsync(user, refreshToken);
            var accessToken = GenerateJwtToken(user);

            return (accessToken, refreshToken);
        }
        catch (InvalidCredentialsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new LoginFailedException("An unexpected error occurred during login.", ex);
        }
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var hashedToken = _hashingService.Hash(refreshToken);
        return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedToken);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        // Validate JWT configuration
        var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
        if (!ValidationUtils.IsBase64String(secretKey)) throw new InvalidOperationException("JWT Key must be a valid Base64 string.");

        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured.");

        var expirationString = jwtSettings["TokenExpirationMinutes"];
        if (!int.TryParse(expirationString, out var expiresInMinutes)) expiresInMinutes = DefaultTokenExpirationMinutes;

        var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        // Create claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        // Generate token
        var key = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
        var algorithm = jwtSettings["Algorithm"] ?? SecurityAlgorithms.HmacSha256;
        var creds = new SigningCredentials(key, algorithm);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    public async Task UpdateRefreshTokenAsync(User user, string refreshToken)
    {
        user.RefreshToken = _hashingService.Hash(refreshToken);
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Example expiry
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokenAsync(User user)
    {
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _context.SaveChangesAsync();
    }

    public async Task LogoutUserAsync(User user)
    {
        await RevokeRefreshTokenAsync(user);
    }

    public async Task ChangePasswordAsync(User user, string newPassword)
    {
        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        await RevokeRefreshTokenAsync(user);
        await _context.SaveChangesAsync();
    }
}

