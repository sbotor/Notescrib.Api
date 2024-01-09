using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Notescrib.Core.Models;
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
        HttpStatusCode statusCode;
        ErrorResponse response;

        if (exception is AppException appException)
        {
            _logger.LogWarning(exception, "An exception occured.");
            statusCode = GetErrorCode(appException);
            response = appException.ToResponse();
        }
        else
        {
            _logger.LogError(exception, "An unhandled exception occured.");
            statusCode = HttpStatusCode.InternalServerError;
            response = ErrorResponse.UnexpectedError;
        }

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private static HttpStatusCode GetErrorCode(AppException exception)
        => exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ForbiddenException => HttpStatusCode.Forbidden,
            DuplicationException => HttpStatusCode.UnprocessableEntity,
            RequestValidationException => HttpStatusCode.BadRequest,
            ServerErrorException => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.BadRequest
        };
}
