using System.ComponentModel.DataAnnotations;

namespace ClipVault.DTOs;

public class RegisterDto
{
    [Required]
    public required string UserName { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?]).{8,}$", 
    ErrorMessage = "Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.")]
    public required string Password { get; set; }
}

public class LoginDto
{
    [Required]
    public required string UserNameOrEmail { get; set; }
    [Required]
    public required string Password { get; set; }
}

