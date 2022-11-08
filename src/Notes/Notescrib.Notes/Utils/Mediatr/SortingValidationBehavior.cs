using MediatR;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Models.Exceptions;

namespace Notescrib.Notes.Utils.Mediatr;

internal class SortingValidationBehavior<TEntity, TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ISortingRequest<TEntity, TResponse>
    where TEntity : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var sorting = request.Sorting;
        if (sorting.IsSafe)
        {
            return await next.Invoke();
        }

        sorting.OrderBy = typeof(TEntity).FindProperty(sorting.OrderBy)?.Name
                          ?? throw new AppException($"Invalid property name '{sorting.OrderBy}'.");

        return await next.Invoke();
    }
}
