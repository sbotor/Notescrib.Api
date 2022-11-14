using MediatR;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Contracts;

internal interface IPagingSortingRequest<TResponse, TSort> : IPagingRequest<TResponse>, ISortingRequest<TSort>
    where TSort : struct, Enum
{
}

internal interface IPagingRequest<TResponse> : IRequest<PagedList<TResponse>>
{
    Paging Paging { get; }
}

internal interface ISortingRequest<TSort>
    where TSort : struct, Enum
{
    Sorting<TSort> Sorting { get; }
}
