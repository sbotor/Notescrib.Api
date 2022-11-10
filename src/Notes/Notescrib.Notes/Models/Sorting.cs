using Notescrib.Notes.Contracts;
using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Models;

public class Sorting<TSort> where TSort : struct, Enum
{
    public TSort OrderBy { get; }
    public SortingDirection Direction { get; }

    public Sorting(TSort orderBy, SortingDirection direction)
    {
        OrderBy = orderBy;
        Direction = direction;
    }
}
