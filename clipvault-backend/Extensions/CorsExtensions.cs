namespace ClipVault.Extensions;

public static class CorsExtensions
{
    public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        if (allowedOrigins == null || allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException("CORS configuration error: No allowed origins specified in the environment.");
        }

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", builder =>
            {
                builder.WithOrigins(allowedOrigins) // Allow specific origins
                       .WithMethods("GET", "POST", "PUT", "DELETE") // Allow specific HTTP methods
                       .WithHeaders("Authorization", "Content-Type") // Allow specific headers
                       .AllowCredentials(); // Allow cookies or credentials if needed
            });
        });
    }
}