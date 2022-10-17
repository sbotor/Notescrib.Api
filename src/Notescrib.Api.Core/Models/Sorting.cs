using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Core.Models;

public class Sorting : ISorting
{
    public string? OrderBy { get; set; }
    public SortingDirection Direction { get; set; } = SortingDirection.Ascending;

    public Sorting(string? orderBy = null, SortingDirection direction = SortingDirection.Ascending)
    {
        OrderBy = orderBy;
        Direction = direction;
    }

    public static ISorting GetDefaultIfEmpty(ISorting sorting, string defaultOrderBy, SortingDirection? direction = null)
        => string.IsNullOrEmpty(sorting.OrderBy)
            ? new Sorting(defaultOrderBy, direction ?? sorting.Direction)
            : sorting;
}
