using Notescrib.Notes.Api.Contracts;
using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Api.Models;

public abstract class PagingSortingRequest<TSort> : IPagingRequest, ISortingRequest<TSort>
    where TSort : struct, Enum
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Paging.DefaultPageSize;
    public TSort OrderBy { get; set; }
    public SortingDirection SortingDirection { get; set; } = SortingDirection.Ascending;
}
