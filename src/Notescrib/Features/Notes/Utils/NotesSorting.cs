using Notescrib.Contracts;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Utils;

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
