using Notescrib.Notes.Features.Folders;

namespace Notescrib.Notes.Features.Notes;

public class NoteContentData
{
    public string Id { get; set; } = null!;
    public string NoteId { get; set; } = null!;
    public string Value { get; set; } = null!;
}

public class NoteContent : NoteContentData
{
    public Note Note { get; set; } = null!;
}

internal class NoteContentFolderLookup : NoteContentData
{
    public IReadOnlyCollection<FolderData> Folders { get; set; } = null!;
}
