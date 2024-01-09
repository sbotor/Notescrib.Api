using FluentValidation;
using MediatR;
using Notescrib.Contracts;
using Notescrib.Models;

namespace Notescrib.Utils.Mediatr;

internal class PagingValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, PagedList<TResponse>>
    where TRequest : IPagingQuery<TResponse>
{
    private readonly IValidator<Paging> _validator;

    public PagingValidationBehavior(IValidator<Paging> validator)
    {
        _validator = validator;
    }

    public async Task<PagedList<TResponse>> Handle(TRequest request, RequestHandlerDelegate<PagedList<TResponse>> next,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request.Paging, cancellationToken);
        return await next.Invoke();
    }
}
