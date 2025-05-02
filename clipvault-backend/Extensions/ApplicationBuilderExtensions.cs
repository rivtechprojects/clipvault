namespace ClipVault.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        // Configure Swagger for development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Configure HTTPS redirection
        app.UseHttpsRedirection();
    }
}
