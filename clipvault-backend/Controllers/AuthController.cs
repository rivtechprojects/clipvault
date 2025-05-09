using Microsoft.AspNetCore.Mvc;
using ClipVault.DTOs;
using clipvault_backend.Services;

namespace clipvault_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var user = await _authService.RegisterUserAsync(dto.UserName, dto.Email, dto.Password);
            return Ok(new { user.UserId, user.UserName, user.Email });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginUserAsync(dto.UserNameOrEmail, dto.Password);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}

