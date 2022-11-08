using Notescrib.Notes.Api.Contracts;
using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Api.Models;

public class PagingSortingRequest : IPagingRequest, ISortingRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = Paging.DefaultPageSize;
    public string? OrderBy { get; set; }
    public SortingDirection SortingDirection { get; set; } = SortingDirection.Ascending;
}
