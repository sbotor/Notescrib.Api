using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Api.Contracts;

public interface ISortingRequest
{
    string? OrderBy { get; set; }
    SortingDirection SortingDirection { get; set; }
}
