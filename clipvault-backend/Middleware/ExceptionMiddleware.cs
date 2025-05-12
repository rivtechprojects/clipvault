using System.Text.Json;
using ClipVault.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace ClipVault.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is NotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = exception.Message
            });
            return;
        }

        if (exception is DbUpdateException dbUpdateException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = "A database update error occurred. Please ensure the data is valid and try again.",
                details = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment() ? dbUpdateException.Message : null
            });
            return;
        }

        if (exception is InvalidOperationException invalidOperationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = "The request was invalid.",
                details = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment() ? invalidOperationException.Message : null
            });
            return;
        }

        if (exception is RegistrationFailedException registrationFailedException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            _logger.LogError(registrationFailedException, "Registration failed.");
            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = "An error occurred during registration. Please try again."
            });
            return;
        }

        if (exception is LoginFailedException loginFailedException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            _logger.LogError(loginFailedException, "Login failed.");
            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = "An error occurred during login. Please try again."
            });
            return;
        }

        if (exception is UserAlreadyExistsException userAlreadyExistsException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            _logger.LogError(userAlreadyExistsException, "User already exists.");
            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = userAlreadyExistsException.Message
            });
            return;
        }

        if (exception is InvalidCredentialsException invalidCredentialsException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            _logger.LogError(invalidCredentialsException, "Invalid username or password.");
            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = context.Response.StatusCode,
                message = invalidCredentialsException.Message
            });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        _logger.LogError(exception, "An unexpected error occurred.");

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode = context.Response.StatusCode,
            message = "An unexpected error occurred. Please contact support if the issue persists.",
            details = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment() ? exception.Message : null
        });
    }
}