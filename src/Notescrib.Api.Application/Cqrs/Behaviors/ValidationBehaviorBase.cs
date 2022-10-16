using FluentValidation;
using MediatR;
using Notescrib.Api.Application.Extensions;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Application.Cqrs.Behaviors;

internal abstract class ValidationBehaviorBase<TRequest, TResponse, TValidation> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TValidation>> _validators;

    public ValidationBehaviorBase(IEnumerable<IValidator<TValidation>> validators)
    {
        _validators = validators;
    }

    public virtual async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next.Invoke();
        }

        var item = GetItem(request);
        var errors = await _validators.ValidateAsync(item);

        if (!errors.Any())
        {
            return await next.Invoke();
        }

        throw new RequestValidationException(errors);
    }

    protected abstract TValidation GetItem(TRequest request);
}
