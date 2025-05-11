using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ClipVault.Utils;

public partial class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password)
        {
            return new ValidationResult("Password is invalid.");
        }

        // Check length
        if (password.Length < 8)
        {
            return new ValidationResult("Password must be at least 8 characters long.");
        }

        // Check complexity
        var hasUpperCase = MyRegex().IsMatch(password);
        var hasLowerCase = MyRegex1().IsMatch(password);
        var hasDigit = MyRegex2().IsMatch(password);
        var hasSpecialChar = MyRegex3().IsMatch(password);

        var errors = new List<string>();

        if (!hasUpperCase)
        {
            errors.Add("- At least one uppercase letter");
        }

        if (!hasLowerCase)
        {
            errors.Add("- At least one lowercase letter");
        }

        if (!hasDigit)
        {
            errors.Add("- At least one number");
        }

        if (!hasSpecialChar)
        {
            errors.Add("- At least one special character (!, @, #, $)");
        }

        if (errors.Count > 0)
        {
            return new ValidationResult("Password must meet the following requirements:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
        }

        return ValidationResult.Success;
    }

    [GeneratedRegex("[A-Z]")]
    private static partial Regex MyRegex();

    [GeneratedRegex("[a-z]")]
    private static partial Regex MyRegex1();

    [GeneratedRegex("\\d")]
    private static partial Regex MyRegex2();
    
    [GeneratedRegex("[!@#$%^&*()_+\\-=\\[\\]{};':\\\"\\|,.<>/?]")]
    private static partial Regex MyRegex3();
}
