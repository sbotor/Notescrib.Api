using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Notescrib.Core.Models.Exceptions;

namespace Notescrib.Core.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unhandled exception occured.");
            await HandleException(e, context);
        }
    }

    private static async Task HandleException(Exception exception, HttpContext context)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occured.";

        if (exception is AppException appException)
        {
            switch (appException)
            {
                case NotFoundException notFound:
                    statusCode = HttpStatusCode.BadRequest;
                    message = GetMessageOrDefault(notFound, "The resource was not found.");
                    break;
                
                case ForbiddenException forbidden:
                    statusCode = HttpStatusCode.Forbidden;
                    message = GetMessageOrDefault(forbidden, "Invalid permissions.");
                    break;
                
                case DuplicationException duplication:
                    statusCode = HttpStatusCode.BadRequest;
                    message = GetMessageOrDefault(duplication, "The resource already exists.");
                    break;

                default:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
            }
        }
        else if (exception is ValidationException validation)
        {
            statusCode = HttpStatusCode.BadRequest;
            
            var errors = validation.Errors.GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    g => g.Distinct()
                        .SelectMany(x => x.ErrorMessage).ToArray());

            message = JsonSerializer.Serialize(errors);
        }

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(message);
    }

    private static string GetMessageOrDefault(AppException exception, string message)
        => !string.IsNullOrEmpty(exception.Message)
            ? exception.Message
            : message;
}
