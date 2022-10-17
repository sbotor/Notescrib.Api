using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Enums;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Contracts;

public class PagingSortingApiRequest : IPaging, ISorting
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = Paging.DefaultPageSize;
    public SortingDirection Direction { get; set; } = SortingDirection.Ascending;
    public string? OrderBy { get; set; }
}
