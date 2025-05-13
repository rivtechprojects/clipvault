using ClipVault.Models;

namespace ClipVault.Interfaces;

public interface IAuthService
{
    Task<User> RegisterUserAsync(string username, string email, string password);
    Task<string> LoginUserAsync(string usernameOrEmail, string password);
    Task<(string AccessToken, string RefreshToken)> LoginUserWithRefreshTokenAsync(string usernameOrEmail, string password);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    Task UpdateRefreshTokenAsync(User user, string refreshToken);
    string GenerateJwtToken(User user);
    Task RevokeRefreshTokenAsync(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task LogoutUserAsync(User user);
    Task ChangePasswordAsync(User user, string newPassword);
    string GenerateRefreshToken();
}
