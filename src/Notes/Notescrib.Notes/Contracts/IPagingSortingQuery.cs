using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Contracts;

internal interface IPagingSortingQuery<TResponse, TSort> : IPagingQuery<TResponse>, ISortingQuery<TSort>
    where TSort : struct, Enum
{
}

internal interface IPagingQuery<TResponse> : IQuery<PagedList<TResponse>>
{
    Paging Paging { get; }
}

internal interface ISortingQuery<TSort>
    where TSort : struct, Enum
{
    Sorting<TSort> Sorting { get; }
}
