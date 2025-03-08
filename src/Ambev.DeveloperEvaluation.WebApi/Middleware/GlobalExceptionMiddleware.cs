using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Erro inesperado ocorreu.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var errorResponse = new
        {
            type = "InternalServerError",
            error = "An unexpected error has occurred.",
            detail = exception.Message
        };

        switch (exception)
        {
            case AuthenticationErrorException authenticateEx:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                errorResponse = new
                {
                    type = "AuthenticationError",
                    error = authenticateEx.Error ?? "Invalid authentication token",
                    detail = authenticateEx.Message ?? "The provided authentication token has expired or is invalid"
                };
                break;
            case ValidationException validationEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse = new
                {
                    type = "ValidationError",
                    error = "Invalid input data",
                    detail = validationEx.Errors.Select(e => e.ErrorMessage).FirstOrDefault() ?? "Validation error."
                };
                break;
            case ResourceNotFoundException resourcenotfoundEx:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                errorResponse = new
                {
                    type = "ResourceNotFound",
                    error = resourcenotfoundEx.Error ?? "Resource not found",
                    detail = resourcenotfoundEx.Message ?? "The requested resource was not found."
                };
                break;
            case InvalidOperationException invalidOpEx:
                context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                errorResponse = new
                {
                    type = "InvalidOperationError",
                    error = "Invalid operation",
                    detail = invalidOpEx.Message
                };
                break;
            case BusinessRuleException businessEx:
                context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                errorResponse = new
                {
                    type = "BusinessRuleError",
                    error = "Business rule error.",
                    detail = businessEx.Message
                };
                break;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
