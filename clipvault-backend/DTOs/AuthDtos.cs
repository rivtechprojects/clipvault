using System.ComponentModel.DataAnnotations;
using ClipVault.Utils;

namespace ClipVault.Dtos;

public class RegisterDto
{
    [UserValidation]
    public required string UserName { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    [PasswordValidation]
    public required string Password { get; set; }
}

public class LoginDto
{
    [Required]
    public required string UserNameOrEmail { get; set; }
    [Required]
    public required string Password { get; set; }
}

