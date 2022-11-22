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
            await HandleException(e, context);
        }
    }

    private async Task HandleException(Exception exception, HttpContext context)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occured.";

        if (exception is AppException appException)
        {
            _logger.LogWarning(exception, "An exception occured.");
            statusCode = HandleAppException(appException, out message);
        }
        else
        {
            _logger.LogError(exception, "An unhandled exception occured.");
        }

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(message);
    }

    private HttpStatusCode HandleAppException(AppException exception, out string message)
    {
        switch (exception)
        {
            case NotFoundException notFound:
                message = GetMessageOrDefault(notFound, "The resource was not found.");
                return HttpStatusCode.BadRequest;
                
            case ForbiddenException forbidden:
                message = GetMessageOrDefault(forbidden, "Invalid permissions.");
                return HttpStatusCode.Forbidden;
                
            case DuplicationException duplication:
                message = GetMessageOrDefault(duplication, "The resource already exists.");
                return HttpStatusCode.UnprocessableEntity;
            
            case RequestValidationException validation:
                message = validation.ToErrorModel().Serialize();
                return HttpStatusCode.BadRequest;

            default:
                message = "Invalid request.";
                return HttpStatusCode.BadRequest;
        }
    }

    private static string GetMessageOrDefault(AppException exception, string message)
        => !string.IsNullOrEmpty(exception.Message)
            ? exception.Message
            : message;
}
