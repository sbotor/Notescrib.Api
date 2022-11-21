using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Features.Notes.Utils;

public enum NotesSorting
{
    Name
}

internal class NotesSortingProvider : ISortingProvider<NotesSorting>
{
    public string GetSortName(NotesSorting value)
        => value switch
        {
            NotesSorting.Name => nameof(Note.Name),
            _ => throw new NotSupportedException()
        };
}
