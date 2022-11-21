using MediatR;
using Microsoft.Extensions.Logging;
using Notescrib.Core.Models.Exceptions;

namespace Notescrib.Core.Cqrs.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var reqName = typeof(TRequest).FullName;
        _logger.LogInformation("Executing request {reqName}.", reqName);

        try
        {
            return await next.Invoke();
        }
        catch (AppException e)
        {
            _logger.LogWarning(e, "Exception when executing {reqName}.", reqName);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "unexpected exception when executing {reqName}.", reqName);
            throw;
        }
    }
}
