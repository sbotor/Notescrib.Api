using FluentValidation;
using MediatR;

namespace Notescrib.Api.Application.Cqrs.Behaviors;

internal class ValidationBehavior<TRequest, TResponse> : ValidationBehaviorBase<TRequest, TResponse, TRequest>
    where TRequest : IRequest<TResponse>
{
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) : base(validators)
    {
    }

    protected override TRequest GetItem(TRequest request) => request;
}
