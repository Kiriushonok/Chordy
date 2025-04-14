using Chordy.BusinessLogic.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using System.Text.Json;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    private static readonly Dictionary<string, (string Title, string Detail)> CheckConstraintMessages = new()
    {
        ["CK_Author_Name_NotEmpty"] = ("Недопустимое значение поля", "Имя автора не может быть пустым."),
        ["CK_Collection_Name_NotEmpty"] = ("Недопустимое значение поля", "Название подборки не может быть пустым.")
    };

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

    private (int StatusCode, string Title, string Detail) HandleDbUpdateException(DbUpdateException dbEx)
    {
        var postgresEx = dbEx.InnerException as PostgresException;
        var message = postgresEx?.Message ?? dbEx.InnerException?.Message ?? dbEx.Message;

        if (postgresEx != null)
        {
            _logger.LogWarning("PostgreSQL ошибка: SqlState={SqlState}, Constraint={Constraint}, Message={MessageText}",
                postgresEx.SqlState, postgresEx.ConstraintName ?? "(null)", postgresEx.MessageText);

            if (postgresEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return ((int)HttpStatusCode.Conflict, "Нарушение уникальности", "Запись с такими данными уже существует.");
            }

            if (postgresEx.SqlState == PostgresErrorCodes.StringDataRightTruncation)
            {
                return ((int)HttpStatusCode.BadRequest, "Превышена длина поля", "Одно из полей превышает допустимую длину.");
            }

            if (postgresEx.SqlState == PostgresErrorCodes.CheckViolation)
            {
                var constraintName = postgresEx.ConstraintName ?? string.Empty;

                if (CheckConstraintMessages.TryGetValue(constraintName, out var msg))
                {
                    return ((int)HttpStatusCode.BadRequest, msg.Title, msg.Detail);
                }

                // Неизвестное ограничение — логируем явно
                _logger.LogWarning("Необработанное ограничение CHECK: {Constraint}", constraintName);

                return ((int)HttpStatusCode.BadRequest, "Ошибка проверки данных",
                    "Одно из введённых значений не прошло проверку. Убедитесь, что поля заполнены корректно.");
            }
        }

        _logger.LogError(dbEx, "Ошибка базы данных: {Message}", message);
        return ((int)HttpStatusCode.InternalServerError, "Ошибка базы данных", "Ошибка при сохранении данных. Проверьте корректность входных данных.");
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Ресурс не найден", exception.Message),
            ArgumentException => ((int)HttpStatusCode.BadRequest, "Ошибка в запросе", exception.Message),
            DuplicationConflictException => ((int)HttpStatusCode.Conflict, "Конфликт дублирования данных", exception.Message),
            DbUpdateException dbEx => HandleDbUpdateException(dbEx),
            _ => ((int)HttpStatusCode.InternalServerError, "Ошибка сервера", "Произошла внутренняя ошибка. Подробнее см. журнал.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://api.chordy.com/errors/{statusCode}"
        };

        var response = JsonSerializer.Serialize(problemDetails);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(response);
    }
}