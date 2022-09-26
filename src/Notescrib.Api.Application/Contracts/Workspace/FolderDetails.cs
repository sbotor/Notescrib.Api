namespace Notescrib.Api.Application.Contracts.Workspace;

public class FolderDetails
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool IsRoot { get; set; }
    public IReadOnlyCollection<NoteDetails> Notes { get; set; } = new List<NoteDetails>();
}
