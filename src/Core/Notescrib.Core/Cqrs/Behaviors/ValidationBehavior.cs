using FluentValidation;
using MediatR;
using Notescrib.Core.Models.Exceptions;

namespace Notescrib.Core.Cqrs.Behaviors;

public class ValidationBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResult> Handle(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators
            .Select(x => x.ValidateAsync(context, cancellationToken)));

        var errors = results.SelectMany(x => x.Errors)
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                x => x.Key,
                x => x.Select(e => e.ErrorMessage).Distinct().ToArray());

        if (errors.Any())
        {
            throw new RequestValidationException(errors);
        }

        return await next.Invoke();
    }
}
