using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Utils;

public enum NotesSorting
{
    Name
}

public class NotesSortingProvider : ISortingProvider<NotesSorting>
{
    public string GetSortName(NotesSorting value)
        => value switch
        {
            NotesSorting.Name => nameof(Note.Name),
            _ => throw new AppException(ErrorCodes.General.InvalidSortingProperty)
        };
}
