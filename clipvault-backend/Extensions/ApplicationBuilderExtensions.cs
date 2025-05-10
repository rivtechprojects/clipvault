using ClipVault.Middleware;

namespace ClipVault.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        // Add middleware
        app.UseMiddleware<ExceptionMiddleware>();
        
        // Configure Swagger for development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Configure HTTPS redirection
        app.UseHttpsRedirection();

        // Configure authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("DefaultPolicy");
    }
}
