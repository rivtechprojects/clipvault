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
                var errors = new Dictionary<string, string[]>
                {
                    { "error", new[] { "Request was invalid. Please check your input and try again." } }
                };
                return new BadRequestObjectResult(new
                {
                    status = 400,
                    message = "Validation failed.",
                    errors
                });
            };
        });
    }
}
