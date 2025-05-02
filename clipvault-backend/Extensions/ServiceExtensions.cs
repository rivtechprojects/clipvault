using ClipVault.Interfaces;
using ClipVault.Services;
using Microsoft.EntityFrameworkCore;
namespace ClipVault.Extensions;


public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Load environment variables from .env file
        DotNetEnv.Env.Load();

        // Build the connection string dynamically
        var connectionStringTemplate = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionStringTemplate))
        {
            throw new InvalidOperationException("The connection string is not defined in the configuration.");
        }

        var connectionString = connectionStringTemplate
            .Replace("${DB_SERVERNAME}", Environment.GetEnvironmentVariable("DB_SERVERNAME") ?? "localhost")
            .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME") ?? "clipvault_dev")
            .Replace("${DB_USERNAME}", Environment.GetEnvironmentVariable("DB_USERNAME") ?? "dev_user")
            .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "dev_password")
            .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT") ?? "5432");

        // Register AppDbContext with AddDbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Map IAppDbContext to AppDbContext
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Register SnippetService
        services.AddScoped<ISnippetService, SnippetService>();

        // Add OpenAPI/Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}
