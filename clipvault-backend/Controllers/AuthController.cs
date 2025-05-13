using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClipVault.Dtos;
using ClipVault.Interfaces;
using System.Security.Claims;

namespace ClipVault.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = await _authService.RegisterUserAsync(dto.UserName, dto.Email, dto.Password);
        return Ok(new { user.UserId, user.UserName, user.Email });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var user = await _authService.GetUserByRefreshTokenAsync(dto.RefreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token." });
        }

        // Generate a new access token and refresh token
        var accessToken = _authService.GenerateJwtToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();
        await _authService.UpdateRefreshTokenAsync(user, newRefreshToken);

        return Ok(new { accessToken, refreshToken = newRefreshToken });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (accessToken, refreshToken) = await _authService.LoginUserWithRefreshTokenAsync(dto.UserNameOrEmail, dto.Password);
        return Ok(new { accessToken, refreshToken });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Get the user from the context (assuming authentication middleware sets the user)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var user = await _authService.GetUserByIdAsync(int.Parse(userId));
        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }

        await _authService.LogoutUserAsync(user);
        return Ok(new { message = "User logged out successfully." });
    }
}

