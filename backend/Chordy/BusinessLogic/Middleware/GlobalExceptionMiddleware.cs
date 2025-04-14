using Chordy.BusinessLogic.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

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
            _logger.LogError(ex, "Произошла необработанная ошибка.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound, // 404
            ArgumentException => (int)HttpStatusCode.BadRequest, // 400
            DuplicationConflictException => (int)HttpStatusCode.Conflict, // 409
            _ => (int)HttpStatusCode.InternalServerError // 500
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                400 => "Ошибка в запросе",
                404 => "Ресурс не найден",
                409 => "Конфликт дублирования данных",
                _ => "Ошибка сервера"
            },
            Detail = exception.Message,
            Type = $"https://api.chordy.com/errors/{statusCode}"
        };

        var response = JsonSerializer.Serialize(problemDetails);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(response);
    }
}
