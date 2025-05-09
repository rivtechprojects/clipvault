using ClipVault.Interfaces;
using ClipVault.Services;
using ClipVault.Filters;

namespace ClipVault.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Load environment variables from .env file
        DotNetEnv.Env.Load();

        // Register AppDbContext with AddDbContext
        services.AddDatabaseConfiguration(configuration);

        // Map IAppDbContext to AppDbContext
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Register Dependencies
        services.AddScoped<ISnippetService, SnippetService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ISnippetMapper, SnippetMapper>();

        // Register ValidationFilter globally for all controllers
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });

        // Add OpenAPI/Swagger
        services.AddSwaggerConfiguration();

        // Add JWT Authentication
        services.AddJwtAuthentication(configuration);

        services.ConfigureApiBehavior();
    }
}
