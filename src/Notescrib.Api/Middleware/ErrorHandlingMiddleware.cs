using System.Net;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private const string DefaultMsg = "An unhandled exception occured.";

    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(e, DefaultMsg);

            await HandleException(e, context);
        }
    }

    private static async Task HandleException(Exception exception, HttpContext context)
    {
        if (exception is AppException appException)
        {
            await SerializeError(context, appException.ToErrorModel(), appException.StatusCode);
        }

        await SerializeError(context, new ErrorModel(exception));
    }

    private static async Task SerializeError(HttpContext context, ErrorModel error, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(error);
    }
}
