using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ClipVault.Utils;

public class UserValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string userName)
        {
            return new ValidationResult("Invalid username.");
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return new ValidationResult("You must enter a username.");
        }

        // Check length
        if (userName.Length < 5)
        {
            return new ValidationResult("Username must be at least 5 characters long.");
        }

        if (userName.Length > 20)
        {
            return new ValidationResult("Username cannot exceed 20 characters.");
        }

        // Check allowed characters
        if (!Regex.IsMatch(userName, "^[a-zA-Z0-9_]+$"))
        {
            return new ValidationResult("Username can only contain letters, numbers, and underscores.");
        }

        return ValidationResult.Success;
    }
}
