using ClipVault.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClipVault.Extensions;

public static class DatabaseExtensions
{
    public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
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

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
    }
}
