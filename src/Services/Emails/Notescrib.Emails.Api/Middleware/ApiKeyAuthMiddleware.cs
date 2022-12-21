using Microsoft.Extensions.Options;

namespace Notescrib.Emails.Api.Middleware;

public class ApiKeyAuthMiddleware
{
    private const string HeaderName = "X-Api-Key";
    
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;
    private readonly AuthSettings _settings;

    public ApiKeyAuthMiddleware(RequestDelegate next, IOptions<AuthSettings> options, ILogger<ApiKeyAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _settings = options.Value;

        if (string.IsNullOrEmpty(_settings.ApiKey))
        {
            throw new InvalidOperationException("No API key provided.");
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var key))
        {
            _logger.LogWarning("Unauthorized access without API key.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        if (!key.Equals(_settings.ApiKey))
        {
            _logger.LogWarning("Unauthorized access with API key {key}.", key!);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        
        await _next.Invoke(context);
    }
}
