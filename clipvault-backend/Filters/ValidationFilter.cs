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

        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors?.Count > 0)
            .ToDictionary(
                field => field.Key,
                field => field.Value?.Errors?.Select(e => e.ErrorMessage).ToArray() 
                    ?? []
            );

        if (errors.Count != 0)
        {
            throw new ValidationException(errors);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}