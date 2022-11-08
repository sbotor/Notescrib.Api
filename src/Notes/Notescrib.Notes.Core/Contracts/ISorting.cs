using Notescrib.Notes.Core.Models.Enums;

namespace Notescrib.Notes.Core.Contracts;

public interface ISorting
{
    SortingDirection Direction { get; }
    string OrderBy { get; }
}
