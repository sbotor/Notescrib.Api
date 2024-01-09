using Notescrib.Models.Enums;

namespace Notescrib.Models;

public class Sorting<TSort> where TSort : struct, Enum
{
    public TSort OrderBy { get; }
    public SortingDirection Direction { get; }

    public Sorting(TSort orderBy = default, SortingDirection direction = SortingDirection.Ascending)
    {
        OrderBy = orderBy;
        Direction = direction;
    }
}
