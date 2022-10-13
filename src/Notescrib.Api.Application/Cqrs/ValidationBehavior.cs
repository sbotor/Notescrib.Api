using FluentValidation;
using MediatR;
using Notescrib.Api.Core;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Cqrs;

internal class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next.Invoke();
        }

        var context = new ValidationContext<TRequest>(request);

        var errors = _validators.Select(validator => validator.Validate(context))
            .SelectMany(x => x.Errors)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propName, messages) => new ValidationError
                {
                    Key = propName,
                    Messages = messages.Distinct().ToList()
                })
            .ToList();

        if (!errors.Any())
        {
            return await next.Invoke();
        }

        throw new RequestValidationException(errors);
    }
}
