using FluentValidation;
using MediatR;
using Notescrib.Notes.Application.Contracts;
using Notescrib.Notes.Application.Models;

namespace Notescrib.Notes.Application.Utils.Mediatr;

internal class PagingValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IPagingRequest<TResponse>
{
    private readonly IValidator<Paging> _validator;

    public PagingValidationBehavior(IValidator<Paging> validator)
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
