using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Core.Contracts;

public interface ISorting
{
    SortingDirection Direction { get; }
    string OrderBy { get; }
}

public interface ISortingNullable
{
    SortingDirection Direction { get; }
    string? OrderBy { get; }
}
