using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Features.Notes.Utils;

public enum NotesSorting
{
    Name,
    Folder
}

internal class NotesSortingProvider : ISortingProvider<NotesSorting>
{
    public string GetSortName(NotesSorting value)
        => value switch
        {
            NotesSorting.Name => nameof(Note.Name),
            NotesSorting.Folder => nameof(Note.Folder),
            _ => throw new NotSupportedException()
        };
}
