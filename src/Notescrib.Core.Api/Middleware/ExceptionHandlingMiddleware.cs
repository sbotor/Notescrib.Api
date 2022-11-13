using System.Net;
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
                    message = !string.IsNullOrEmpty(notFound.Message)
                        ? notFound.Message
                        : "The resource was not found.";
                    break;
                
                case ForbiddenException forbidden:
                    statusCode = HttpStatusCode.Forbidden;
                    message = !string.IsNullOrEmpty(forbidden.Message)
                        ? forbidden.Message
                        : "Invalid permissions.";
                    break;
                
                default:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
            }
        }

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(message);
    }
}
