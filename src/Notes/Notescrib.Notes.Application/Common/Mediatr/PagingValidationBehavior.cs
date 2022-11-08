using FluentValidation;
using MediatR;
using Notescrib.Notes.Application.Common.Contracts;
using Notescrib.Notes.Core.Contracts;

namespace Notescrib.Notes.Application.Common.Mediatr;

internal class PagingValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IPagingRequest<TResponse>
{
    private readonly IValidator<IPaging> _validator;

    public PagingValidationBehavior(IValidator<IPaging> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request.Paging, cancellationToken);
        return await next.Invoke();
    }
}
