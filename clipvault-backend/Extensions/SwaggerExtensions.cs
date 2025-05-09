using Microsoft.OpenApi.Models;

namespace ClipVault.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClipVault API", Version = "v1" });
        });
    }
}
