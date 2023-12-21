using Notescrib.Models.Enums;

namespace Notescrib.WebApi.Contracts;

public interface ISortingRequest<TSort> where TSort : struct, Enum
{
    TSort OrderBy { get; set; }
    SortingDirection SortingDirection { get; set; }
}
