using MediatR;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Contracts;

internal interface IPagingSortingRequest<TEntity, out TResponse> : IPagingRequest<TResponse>, ISortingRequest<TEntity, TResponse>
    where TEntity : class
{
}

internal interface IPagingRequest<out TResponse> : IRequest<TResponse>
{
    Paging Paging { get; }
}

internal interface ISortingRequest<TEntity, out TResponse> : IRequest<TResponse>
    where TEntity : class
{
    Sorting Sorting { get; }
}
