using FluentValidation;
using MediatR;
using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Application.Cqrs.Behaviors;

internal class PagingValidationBehavior<TRequest, TResponse> : ValidationBehaviorBase<TRequest, TResponse, IPaging>
    where TRequest : IRequest<TResponse>, IPagingRequest
{
    public PagingValidationBehavior(IEnumerable<IValidator<IPaging>> validators) : base(validators)
    {
    }

    protected override IPaging GetItem(TRequest request) => request.Paging;
}
