using MediatR;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Contracts;

internal interface IPagingSortingRequest<TSort, TResponse> : IPagingRequest<TResponse>, ISortingRequest<TSort, TResponse>
    where TSort : struct, Enum
{
}

internal interface IPagingRequest<TResponse> : IRequest<PagedList<TResponse>>
{
    Paging Paging { get; }
}

internal interface ISortingRequest<TSort, out TResponse> : IRequest<TResponse>
    where TSort : struct, Enum
{
    Sorting<TSort> Sorting { get; }
}
