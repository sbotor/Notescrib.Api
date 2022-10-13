using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Notescrib.Api.Core.Exceptions;

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
        var feature = context.Features.Get<IExceptionHandlerFeature>();

        if (feature == null)
        {
            await _next.Invoke(context);
            return;
        }

        var exception = feature.Error;
        _logger.LogError(exception, DefaultMsg);

        if (exception is RequestValidationException validationException)
        {
            await SerializeError(context, validationException.SerializeErrors(), HttpStatusCode.BadRequest);
            return;
        }

        await SerializeError(context);
    }

    private static async Task SerializeError(HttpContext context, object? content = null, HttpStatusCode code = HttpStatusCode.InternalServerError)
    {
        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsJsonAsync(content ?? DefaultMsg);
    }
}
