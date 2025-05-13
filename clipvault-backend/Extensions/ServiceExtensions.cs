using ClipVault.Interfaces;
using ClipVault.Services;
using Microsoft.AspNetCore.Identity;
using ClipVault.Models;
using ClipVault.Mappers;
using ClipVault.Utils;

namespace ClipVault.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register AppDbContext with AddDbContext
        services.AddDatabaseConfiguration(configuration);

        // Map IAppDbContext to AppDbContext
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Register Dependencies
        services.AddScoped<ISnippetService, SnippetService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ISnippetMapper, SnippetMapper>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHashingService, SHA256HashingService>();

        // Register IPasswordHasher
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Add OpenAPI/Swagger
        services.AddSwaggerConfiguration();

        // Add JWT Authentication
        services.AddJwtAuthentication(configuration);

        services.ConfigureApiBehavior();
        services.AddCorsPolicy(configuration);
    }
}
