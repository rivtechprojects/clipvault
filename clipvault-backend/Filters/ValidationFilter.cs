using ClipVault.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClipVault.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState == null || context.ModelState.IsValid)
        {
            return;
        }

        var errors = new Dictionary<string, string[]>
        {
            { "error", new[] { "Request was invalid. Please check your input and try again." } }
        };
        throw new ValidationException(errors);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}