using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClipVault.Dtos;
using ClipVault.Interfaces;

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
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginUserAsync(dto.UserNameOrEmail, dto.Password);
        return Ok(new { token });
    }
}

