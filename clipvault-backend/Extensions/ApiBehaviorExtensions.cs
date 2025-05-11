using Microsoft.AspNetCore.Mvc;

namespace ClipVault.Extensions;

public static class ApiBehaviorExtensions
{
    public static void ConfigureApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        ms => ms.Key,
                        ms => ms.Value?.Errors?.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );

                return new BadRequestObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = "Validation failed.",
                    errors
                });
            };
        });
    }
}
