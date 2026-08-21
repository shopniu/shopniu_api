
using System.Net;
using System.Text.Json;
using Shopniu_api.Aplication.Common;
using Shopniu_api.Domain.Exceptions;
using Shopniu_api.Domain.Exceptions.Common;

namespace Shopniu_api.Infrastructure.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode == HttpStatusCode.InternalServerError)

            _logger.LogError(exception, "An unhandled exception occurred.");

        else
            _logger.LogWarning(exception, "A handled exception occurred.");

        var problemDetails = new
        {
            type = $"https://httpstatusesda.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail = exception.Message,
            errors = exception is ValidationsException validationException ? validationException.Errors : null
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(jsonResponse);
    }


    private static (HttpStatusCode statusCode, string title) MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource Not Found"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
            ValidationsException => (HttpStatusCode.BadRequest, "Validation Error"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict Error"),
            BusinessRuleException => (HttpStatusCode.BadRequest, "Business Rule Violation"),

            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };
    }
}