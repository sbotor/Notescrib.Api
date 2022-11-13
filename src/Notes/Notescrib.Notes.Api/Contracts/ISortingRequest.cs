using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Api.Contracts;

public interface ISortingRequest<TSort> where TSort : struct, Enum
{
    TSort OrderBy { get; set; }
    SortingDirection SortingDirection { get; set; }
}
