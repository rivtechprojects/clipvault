using ClipVault.Models;

namespace ClipVault.Interfaces;

public interface IAuthService
{
    Task<User> RegisterUserAsync(string username, string email, string password);
    Task<string> LoginUserAsync(string usernameOrEmail, string password);
}
